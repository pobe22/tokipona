let currentSentence = null;
let selectedWords = [];
let stats = { correct: 0, incorrect: 0, streak: 0 };
let draggedElement = null;

// Initialisierung
document.addEventListener('DOMContentLoaded', function () {
    initializeDragAndDrop();
    loadQuestion();

    // Schwierigkeitsgrad-Änderung
    document.getElementById('difficulty').addEventListener('change', function () {
        loadQuestion();
    });
});

// Frage laden
async function loadQuestion() {
    showLoading(true);
    hideElements(['quizContainer', 'feedbackSection']);

    try {
        const difficulty = document.getElementById('difficulty').value;
        const response = await fetch(`/SentenceExercise/GenerateSentence?difficulty=${difficulty}`);

        if (!response.ok) throw new Error('Fehler beim Laden der Frage');

        currentSentence = await response.json();
        displayQuestion(currentSentence);

    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Laden der Frage. Bitte versuche es erneut.');
    } finally {
        showLoading(false);
        document.getElementById('quizContainer').style.display = 'block';
    }
}

// Frage anzeigen
function displayQuestion(sentence) {
    document.getElementById('questionText').textContent = sentence.germanSentence;
    document.getElementById('correctAnswer').textContent = sentence.tokiPonaSentence;
    document.getElementById('manualInput').value = '';

    // Word Pool erstellen
    const wordPool = document.getElementById('wordPool');
    wordPool.innerHTML = '';

    if (sentence.wordPool && sentence.wordPool.length > 0) {
        sentence.wordPool.forEach((word, index) => {
            const chip = createWordChip(word, index);
            wordPool.appendChild(chip);
        });
    }

    // Answer Display zurücksetzen
    resetAnswerDisplay();
    selectedWords = [];

    // Buttons zurücksetzen
    document.getElementById('checkBtn').style.display = 'inline-block';
    document.getElementById('nextBtn').style.display = 'none';
    document.getElementById('feedbackSection').style.display = 'none';
}

// Word Chip erstellen
function createWordChip(word, index) {
    const chip = document.createElement('div');
    chip.className = 'word-chip';
    chip.textContent = word;
    chip.draggable = true;
    chip.dataset.word = word;
    chip.dataset.index = index;

    chip.addEventListener('dragstart', handleDragStart);
    chip.addEventListener('dragend', handleDragEnd);
    chip.addEventListener('click', function () {
        addWordToAnswer(word, index);
    });

    return chip;
}

// Drag and Drop initialisieren
function initializeDragAndDrop() {
    const dropzone = document.getElementById('answerDropzone');

    dropzone.addEventListener('dragover', function (e) {
        e.preventDefault();
        this.classList.add('dragover');
    });

    dropzone.addEventListener('dragleave', function () {
        this.classList.remove('dragover');
    });

    dropzone.addEventListener('drop', function (e) {
        e.preventDefault();
        this.classList.remove('dragover');

        if (draggedElement) {
            const word = draggedElement.dataset.word;
            const index = draggedElement.dataset.index;
            addWordToAnswer(word, index);
        }
    });
}

function handleDragStart(e) {
    draggedElement = this;
    this.style.opacity = '0.5';
}

function handleDragEnd(e) {
    this.style.opacity = '1';
    draggedElement = null;
}

// Wort zur Antwort hinzufügen
function addWordToAnswer(word, index) {
    const chip = document.querySelector(`.word-chip[data-index="${index}"]`);
    if (chip && !chip.classList.contains('used')) {
        selectedWords.push(word);
        chip.classList.add('used');
        updateAnswerDisplay();
    }
}

// Antwort-Display aktualisieren
function updateAnswerDisplay() {
    const display = document.getElementById('answerDisplay');

    if (selectedWords.length === 0) {
        display.innerHTML = '<span class="placeholder">Ziehe Wörter hierher oder tippe deine Antwort ein...</span>';
    } else {
        display.innerHTML = '';
        selectedWords.forEach((word, index) => {
            const chip = document.createElement('div');
            chip.className = 'word-chip';
            chip.textContent = word;
            chip.onclick = function () {
                removeWordFromAnswer(index);
            };
            display.appendChild(chip);
        });
    }
}

// Wort aus Antwort entfernen
function removeWordFromAnswer(index) {
    const word = selectedWords[index];
    selectedWords.splice(index, 1);

    // Word Pool Chip wieder aktivieren
    const chips = document.querySelectorAll('.word-chip');
    chips.forEach(chip => {
        if (chip.dataset.word === word && chip.classList.contains('used')) {
            chip.classList.remove('used');
        }
    });

    updateAnswerDisplay();
}

// Antwort überprüfen
async function checkAnswer() {
    const manualInput = document.getElementById('manualInput').value.trim();
    const userAnswer = manualInput || selectedWords.join(' ');

    if (!userAnswer) {
        alert('Bitte gib eine Antwort ein!');
        return;
    }

    document.getElementById('checkBtn').disabled = true;
    showLoading(true);

    try {
        const response = await fetch('/SentenceExercise/CheckAnswer', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                userAnswer: userAnswer,
                correctAnswer: currentSentence.tokiPonaSentence
            })
        });

        if (!response.ok) throw new Error('Fehler bei der Überprüfung');

        const result = await response.json();
        displayFeedback(result.isCorrect, result.feedback);
        updateStats(result.isCorrect);

    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler bei der Überprüfung. Bitte versuche es erneut.');
    } finally {
        showLoading(false);
        document.getElementById('checkBtn').disabled = false;
    }
}

// Feedback anzeigen
function displayFeedback(isCorrect, feedback) {
    const feedbackSection = document.getElementById('feedbackSection');
    const feedbackIcon = document.getElementById('feedbackIcon');
    const feedbackMessage = document.getElementById('feedbackMessage');
    const aiExplanation = document.getElementById('aiExplanation');

    feedbackSection.style.display = 'block';
    feedbackSection.className = 'feedback-section ' + (isCorrect ? 'correct' : 'incorrect');

    feedbackIcon.textContent = isCorrect ? '✓' : '✗';
    feedbackMessage.textContent = isCorrect ? 'Richtig!' : 'Leider falsch';
    aiExplanation.textContent = feedback;

    // Korrekte Antwort anzeigen wenn falsch
    if (!isCorrect) {
        document.getElementById('correctAnswer').style.display = 'block';
    }

    // Buttons umschalten
    document.getElementById('checkBtn').style.display = 'none';
    document.getElementById('nextBtn').style.display = 'inline-block';
}

// Statistiken aktualisieren
function updateStats(isCorrect) {
    if (isCorrect) {
        stats.correct++;
        stats.streak++;
    } else {
        stats.incorrect++;
        stats.streak = 0;
    }

    document.getElementById('correctCount').textContent = stats.correct;
    document.getElementById('incorrectCount').textContent = stats.incorrect;
    document.getElementById('streakCount').textContent = stats.streak;
}

// Nächste Frage
function nextQuestion() {
    loadQuestion();
}

// Hinweis anzeigen
function showHint() {
    const hint = `Die korrekte Antwort beginnt mit: ${currentSentence.tokiPonaSentence.split(' ')[0]}`;
    alert(hint);
}

// Hilfsfunktionen
function showLoading(show) {
    document.getElementById('loadingSpinner').style.display = show ? 'block' : 'none';
}

function hideElements(ids) {
    ids.forEach(id => {
        const element = document.getElementById(id);
        if (element) element.style.display = 'none';
    });
}

function resetAnswerDisplay() {
    const display = document.getElementById('answerDisplay');
    display.innerHTML = '<span class="placeholder">Ziehe Wörter hierher oder tippe deine Antwort ein...</span>';
}

// Beispiel: Benutzer-Auswahl im Frontend
async function loadSentence(difficulty, forceNew = false) {
    const response = await fetch(
        `/SentenceExercise/GenerateSentence?difficulty=${difficulty}&forceNew=${forceNew}`
    );
    const data = await response.json();

    console.log(`Source: ${data.source}, Tokens used: ${data.tokensUsed}`);

    if (data.source === 'database') {
        showInfo('♻️ Bestehender Satz wiederverwendet - keine Kosten!');
    } else {
        showWarning(`⚠️ Neuer Satz generiert - ${data.tokensUsed} Tokens verbraucht`);
    }

    displaySentence(data.sentence);
}

// Button-Handler
document.getElementById('btnNewSentence').onclick = () => loadSentence('beginner', true);
document.getElementById('btnExistingSentence').onclick = () => loadSentence('beginner', false);

