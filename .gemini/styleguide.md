## 以下のファイルは全て英文で書かれていますが、コードレビューは全て日本語で行ってください。

### ✅ **Technical Requirements**

* **Code Quality**

    * Follow established coding standards (e.g., PEP8, Google Style Guide).
    * Ensure consistent and meaningful naming conventions for variables, functions, and classes.
    * Eliminate unused code, variables, and imports.

* **Readability & Maintainability**

    * Ensure comments and documentation are present, concise, and useful.
    * Add explanations for complex logic or non-obvious decisions.
    * Functions and classes should follow the Single Responsibility Principle (SRP).

* **Security**

    * Validate and sanitize user inputs and external data properly.
    * Avoid hard-coded secrets, credentials, or tokens.
    * Watch for security risks like SQL injection, XSS, etc.

* **Performance**

    * Identify and optimize potential bottlenecks.
    * Avoid redundant computations or heavy operations in tight loops.

* **Testing & CI/CD**

    * Verify that unit and integration tests are provided and meaningful.
    * Ensure high test coverage, especially for critical logic.
    * Confirm that CI/CD pipelines are set up and test logs are accessible.

* **Dependencies & Structure**

    * Minimize unnecessary external dependencies.
    * Confirm dependency versions are locked (`requirements.txt`, `package.json`, etc.).
    * Maintain clear project structure and modularity.

---

### 🧭 **Non-Technical Requirements**

* **Review Prompt Context**

    * Provide a clear description of the code's purpose and where it fits into the overall system.
    * Indicate specific areas or concerns to focus on (e.g., readability, performance, architecture).

* **Language & Style**

  * Post a code review in Japanese.
  * All comments should be simple and concise, and focus on the code's functionality, readability, and maintainability.

* **Tone & Communication**

    * Feedback should be constructive, polite, and solution-oriented.
    * Tailor the level of explanation based on the developer's skill level (e.g., beginner-friendly or advanced).

* **Categorization of Comments**

    * Classify suggestions as:

        * `Critical` (must fix)
        * `Recommended` (should fix)
        * `Optional` (nice to have)
    * Keep the number of comments manageable and focused.

* **Review Iteration Handling**

    * For follow-up reviews, check if previous feedback has been addressed.
    * Emphasize diffs and changes since the last review.

* **Review Traceability**

    * Ensure the ability to track and reference past review comments and discussions.

* **Ethical Considerations**

    * Avoid any biased, discriminatory, or offensive language.
    * Be cautious with Personally Identifiable Information (PII) or sensitive content in examples.
