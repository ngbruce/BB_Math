## MODIFIED Requirements
### Requirement: LV5 Difficulty Operations
The system SHALL generate LV5 difficulty problems (100以上加减乘除带余数) with operands in range 1-999 and equal probability for all five operation types (加、减、乘、无余数除、有余数除). Note: "100以上" means the operand range is extended to three-digit numbers (1-999), not enforcing operands >= 100. Division operands and quotients may be less than 100, which complies with the overall "1-999" range requirement.

#### Scenario: LV5 addition operations
- **GIVEN** LV5 difficulty level
- **WHEN** addition problem is generated
- **THEN** operands are 100-999, result is 200-1998, result must not exceed 2000

#### Scenario: LV5 subtraction operations
- **GIVEN** LV5 difficulty level
- **WHEN** subtraction problem is generated
- **THEN** operands are 100-999, result is 0-899

#### Scenario: LV5 multiplication operations
- **GIVEN** LV5 difficulty level
- **WHEN** multiplication problem is generated
- **THEN** operands are two-digit numbers (10-99), result is 100-9801

#### Scenario: LV5 division without remainder
- **GIVEN** LV5 difficulty level
- **WHEN** division without remainder problem is generated
- **THEN** divisor and quotient are two-digit numbers (10-99), result is 100-9801

#### Scenario: LV5 division with remainder
- **GIVEN** LV5 difficulty level
- **WHEN** division with remainder problem is generated
- **THEN** divisor and quotient are two-digit numbers (10-99), result is 100-9801, remainder < divisor
