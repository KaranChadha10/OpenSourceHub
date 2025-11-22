# Contributing to OpenSourceHub

## Branching Strategy

We use a structured branching workflow to ensure stability.

- **`main`**: Production-ready code. Protected.
- **`dev`**: Integration branch. All new features merge here first.
- **`feature/*`**: Individual feature branches (e.g., `feature/auth-setup`).

## Workflow

1.  **Checkout `dev`**: Always start from the latest `dev` branch.
    ```bash
    git checkout dev
    git pull origin dev
    ```

2.  **Create a Feature Branch**:
    ```bash
    git checkout -b feature/your-feature-name
    ```

3.  **Commit Changes**:
    ```bash
    git add .
    git commit -m "feat: add new feature"
    ```

4.  **Push and PR**:
    - Push your branch to origin.
    - Open a Pull Request (PR) targeting the **`dev`** branch.
