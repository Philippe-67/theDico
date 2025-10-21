# Spécification API
## Auth
### POST /api/auth/register
Body:
{ "username": "string", "password": "string" }
Response:
{ "message": "User created" }
### POST /api/auth/login
Body:
{ "username": "string", "password": "string" }
Response:
{ "token": "JWT" }
## Words
### GET /api/words
Headers: Authorization: Bearer <token>
### POST /api/words
Body:
{ "french": "chat", "english": "cat" }
### DELETE /api/words/{id}
## Quiz
### GET /api/quiz
Retourne une liste de mots aléatoires pour les quiz.
