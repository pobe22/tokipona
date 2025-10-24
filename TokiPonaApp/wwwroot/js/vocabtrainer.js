let vocab = [];
let currentQuestion = 0;

document.addEventListener('DOMContentLoaded', function () {
    loadVocabulary();
    setupLoginModal();
});

function loadVocabulary() {
    fetch('/VocabTrainer/GetVocabulary')
        .then(response => response.json())
        .then(data => {
            vocab = data;
            loadQuestion();
        })
        .catch(error => console.error('Fehler beim Laden der Vokabeln:', error));
}

function loadQuestion() {
    if (vocab.length === 0) return;

    const question = vocab[currentQuestion];
    document.querySelector('.question').textContent = question.tokiPonaWord;

    const optionsContainer = document.querySelector('.options');
    optionsContainer.innerHTML = '';

    question.options.forEach(option => {
        const button = document.createElement('button');
        button.textContent = option;
        button.onclick = () => checkAnswer(option, question.germanTranslation);
        optionsContainer.appendChild(button);
    });
}

function checkAnswer(selected, correct) {
    const feedback = document.querySelector('.feedback');
    feedback.style.display = 'block';

    if (selected === correct) {
        feedback.textContent = 'Richtig!';
        feedback.className = 'feedback correct';
        submitAnswer(true);
    } else {
        feedback.textContent = `Falsch! Die richtige Antwort ist: ${correct}`;
        feedback.className = 'feedback incorrect';
        submitAnswer(false);
    }
}

function nextQuestion() {
    currentQuestion = (currentQuestion + 1) % vocab.length;
    document.querySelector('.feedback').style.display = 'none';
    loadQuestion();
}

function submitAnswer(isCorrect) {
    fetch('/VocabTrainer/SubmitAnswer', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ isCorrect })
    });
}

function setupLoginModal() {
    const loginLink = document.getElementById('loginLink');
    const logoutLink = document.getElementById('logoutLink');
    const modal = document.getElementById('loginModal');
    const closeBtn = document.querySelector('.close');
    const loginForm = document.getElementById('loginForm');

    if (loginLink) {
        loginLink.onclick = () => modal.style.display = 'block';
    }

    if (closeBtn) {
        closeBtn.onclick = () => modal.style.display = 'none';
    }

    if (loginForm) {
        loginForm.onsubmit = function (e) {
            e.preventDefault();
            const username = document.getElementById('username').value;

            fetch('/Account/Login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `username=${encodeURIComponent(username)}`
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        location.reload();
                    }
                });
        };
    }

    if (logoutLink) {
        logoutLink.onclick = function () {
            fetch('/Account/Logout', { method: 'POST' })
                .then(() => location.reload());
        };
    }
}
