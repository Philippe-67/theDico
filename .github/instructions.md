# Instructions de Base pour le Développement

## les Pratiques de Coding

### 1. Principes SOLID
- **S** : Responsabilité unique (Single Responsibility Principle)
- **O** : Ouvert/Fermé (Open/Closed Principle)
- **L** : Substitution de Liskov (Liskov Substitution Principle)
- **I** : Ségrégation des interfaces (Interface Segregation Principle)
- **D** : Injection de dépendances (Dependency Inversion Principle)

### 2. Architecture en Couches
- **Séparez** la logique métier, la présentation et l’accès aux données.
- Utilisez des interfaces pour découpler les couches.

### 3. concernant les tests
- **Écrivez des tests unitaires** pour chaque fonctionnalité.
- Utilisez des frameworks de tests adaptés à votre langage.
- Pratiquez le TDD (Test Driven Development) si possible.

### 4. Revue de Code
- Faites relire votre code par un pair avant de merger.
- Respectez les conventions de nommage et de style du projet.

### 5. Documentation
- Commentez le code complexe.
- Maintenez un README à jour.

### 6. Gestion de Version
- Utilisez des branches pour chaque fonctionnalité ou correction.
- Commitez souvent avec des messages clairs et concis.

---

Respectez ces pratiques pour garantir un code maintenable, évolutif et de qualité.

### 7. Minimal-API
- **N'utilisez pas les minimal-API** : Préférez l'approche classique avec contrôleurs et Startup/Program configurés pour l'injection de dépendances et le routage.