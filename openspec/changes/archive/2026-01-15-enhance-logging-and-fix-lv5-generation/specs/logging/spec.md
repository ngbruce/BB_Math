## ADDED Requirements
### Requirement: 答案日志输出
The system SHALL print the correct answer to the test log (bbmath.dat) for all problem types when a problem is generated.

#### Scenario: Addition answer log
- **WHEN** an addition problem is generated with operands 25 and 37
- **THEN** the system shall log "Addition:25 + 37 = 62" at Info level

#### Scenario: Subtraction answer log
- **WHEN** a subtraction problem is generated with operands 85 and 27
- **THEN** the system shall log "Subtraction:85 - 27 = 58" at Info level

#### Scenario: Multiplication answer log
- **WHEN** a multiplication problem is generated with operands 7 and 8
- **THEN** the system shall log "Multiplication:7 × 8 = 56" at Info level

#### Scenario: Division without remainder answer log
- **WHEN** a division without remainder problem is generated with operands 72 and 8
- **THEN** the system shall log "DivisionNoRemainder:72 ÷ 8 = 9" at Info level

#### Scenario: Division with remainder answer log
- **WHEN** a division with remainder problem is generated with operands 47 and 6
- **THEN** the system shall log "DivisionWithRemainder:47 ÷ 6 = 7......5" at Info level
