
# Toki Pona Quiz

## Inhaltsverzeichnis

1. [Projektbeschreibung](#projektbeschreibung)
2. [Installationsanweisungen](#installationsanweisungen)
3. [Verwendungszweck](#verwendungszweck)
4. [Dateistruktur](#dateistruktur)
5. [Contributing](#contributing)
6. [Lizenz](#lizenz)

## Projektbeschreibung

Dies ist ein Toki Pona Quiz-Projekt, das zwei Hauptfunktionen bietet:
1. Ein Quiz, bei dem Benutzer die Bedeutung eines Toki Pona Satzes in Deutsch erraten müssen.
2. Eine Übung, bei der Benutzer deutsche Wörter in der richtigen Reihenfolge anordnen müssen, um einen gegebenen Toki Pona Satz zu übersetzen.

## Installationsanweisungen

1. Klone das Repository:
    ```bash
    git clone https://github.com/dein-benutzername/toki-pona-quiz.git
    ```
2. Navigiere in das Projektverzeichnis:
    ```bash
    cd toki-pona-quiz
    ```
3. Stelle sicher, dass du einen Webserver hast. Du kannst zum Beispiel `live-server` verwenden:
    ```bash
    npm install -g live-server
    ```
4. Starte den Webserver:
    ```bash
    live-server
    ```

## Verwendungszweck

1. Öffne `index.html` in deinem Webbrowser.
2. Klicke auf "Startseite", um das Toki Pona Quiz zu starten.
3. Klicke auf "Stats", um die Statistiken anzusehen.
4. Klicke auf "Anmelden", um dich anzumelden und deine Fortschritte zu verfolgen.
5. Navigiere zu `saetzeueben.html`, um die Übungssätze zu absolvieren.

## Dateistruktur

```
toki-pona-quiz/
├── index.html
├── saetzeueben.html
├── stats.html
├── style.css
├── vokabeln.json
└── beispielsaetze.json
```

- `index.html`: Die Hauptseite des Toki Pona Quiz.
- `saetzeueben.html`: Die Seite zum Üben von Toki Pona Sätzen.
- `stats.html`: Die Statistikseite.
- `style.css`: Die CSS-Datei für das Styling der Seiten.
- `vokabeln.json`: Die JSON-Datei mit den Vokabeln.
- `beispielsaetze.json`: Die JSON-Datei mit den Beispielsätzen.

## Contributing

Beiträge sind willkommen! Bitte erstelle einen Fork des Repositories und erstelle dann einen Pull-Request mit deinen Änderungen.

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert.
