## MODIFIED Requirements
### Requirement: 全对通关额外奖励
The system SHALL award bonus coins when user completes all questions without any errors.

#### Scenario: Full score bonus with 5 questions
- **GIVEN** user completes all questions and error count is 0, initial question count is 5
- **WHEN** practice ends
- **THEN** bonus coins = 5 × 0.5 = 2 (rounded down)

#### Scenario: Full score bonus with 10 questions
- **GIVEN** user completes all questions and error count is 0, initial question count is 10
- **WHEN** practice ends
- **THEN** bonus coins = 10 × 0.5 = 5

#### Scenario: Full score bonus with 15 questions
- **GIVEN** user completes all questions and error count is 0, initial question count is 15
- **WHEN** practice ends
- **THEN** bonus coins = 15 × 0.5 = 7 (rounded down)

#### Scenario: Full score bonus with 20 questions
- **GIVEN** user completes all questions and error count is 0, initial question count is 20
- **WHEN** practice ends
- **THEN** bonus coins = 20 × 0.5 = 10
