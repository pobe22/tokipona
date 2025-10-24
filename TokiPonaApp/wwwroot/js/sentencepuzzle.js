let sentences = [];
let currentSentence = null;
const commonGermanWords = ['ich', 'du', 'er', 'sie', 'es', 'wir', 'ihr', 'ist', 'sind', 'hat', 'haben'];

document.addEventListener('DOMContentLoaded', function () {
    init();
});

function init() {
    fetch('/SentencePuzzle/GetPuzzleData')
        .then(response => response.json())
        .then(data => {
            sentences = data;
            loadSentence();
        })
        .catch(error => {
            console.error('Fehler beim Laden der Sätze:', error);
        });
}

function loadSentence() {
    if (sentences.length === 0) return;

    currentSentence = sentences[Math.floor(Math.random() * sentences.length)];

    document.querySelector('.sentence').textContent = currentSentence.tokiPonaSentence;

    const wordPool = createWordPool(currentSentence.germanSentence);
    const wordPoolElement = document.querySelector('.word-pool');
    wordPoolElement.innerHTML = '';

    wordPool.forEach(word => {
        const span = document.createElement('span');
        span.textContent = word;
        span.className = 'word';
        span.onclick = () => addWordToInput(word);
        wordPoolElement.appendChild(span);
    });

    document.querySelector('.user-input').innerHTML = '';
}

function createWordPool(sentence) {
    const words = sentence.split(' ');
    const distractors = commonGermanWords.filter(w => !words.includes(w))
        .sort(() => 0.5 - Math.random())
        .slice(0, 3);

    return [...words, ...distractors].sort(() => 0.5 - Math.random());
}

function addWordToInput(word) {
    const userInput = document.querySelector('.user-input');
    const span = document.createElement('span');
    span.textContent = word;
    span.className = 'selected-word';
    span.onclick = () => span.remove();
    userInput.appendChild(span);
}

function checkAnswer() {
    const userInput = document.querySelector('.user-input');
    const userAnswer = Array.from(userInput.children)
        .map(span => span.textContent)
        .join(' ');

    if (userAnswer === currentSentence.germanSentence) {
        alert('Richtig!');
        loadSentence();
    } else {
        alert(`Falsch! Die richtige Antwort ist: ${currentSentence.germanSentence}`);
    }
}
