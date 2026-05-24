## MODIFIED Requirements

### Requirement: 惩罚题目仅追加到当前错题型
(SHALL) 答错惩罚增加的题目数量应仅追加到当前答错的题型，而不是重新均匀分配给所有题型。

#### Scenario: 答错将惩罚题目追加到当前题型
- **GIVEN** 当前题型为整数乘法，剩余题目 10 题，该题型剩余 3 题，惩罚 punishment=2
- **WHEN** 用户答错一题
- **THEN** 总剩余题目增加为 12 题
- **AND** 整数乘法题型的剩余题目增加为 5 题（3 + 2）
- **AND** 其他题型的剩余题目不变

#### Scenario: 答错后不重新初始化题型池
- **GIVEN** 题型池已消耗部分题型
- **WHEN** 用户答错一题
- **THEN** 题型池的已有分配不被清除
- **AND** 答题顺序和剩余分布不受惩罚扰动

## ADDED Requirements

### Requirement: 错误惩罚机制
(SHALL) 用户答错题目时，系统应增加惩罚题目，惩罚数量可配置。惩罚题目应仅追加到当前答错题型。

#### Scenario: 答错增加惩罚题目
- **GIVEN** 当前剩余题目 10 题，错误惩罚 punishment=2
- **WHEN** 用户答错一题
- **THEN** 剩余题目增加为 12 题（10 + 2）
- **AND** 惩罚题目数仅追加到当前错题型

#### Scenario: 惩罚题目统计
- **GIVEN** 整数乘法题型，初始 TotalQty=20
- **WHEN** 用户答错 2 题（每次惩罚 2 题）
- **THEN** WrongQty=2, TotalQty=24（20 + 2×2）

#### Scenario: 惩罚后继续答题
- **GIVEN** 用户答错后剩余题目增加
- **WHEN** 生成新题目
- **THEN** 用户必须完成惩罚增加的题目才能通关

#### Scenario: 惩罚配置为 0
- **GIVEN** 错误惩罚 punishment=0
- **WHEN** 用户答错题目
- **THEN** 剩余题目数不变，只记录错误数

#### Scenario: 惩罚不影响金币
- **GIVEN** 用户答错题目，触发惩罚机制
- **WHEN** 增加惩罚题目
- **THEN** 金币数不变化（惩罚只增加题目数，不扣除金币）
