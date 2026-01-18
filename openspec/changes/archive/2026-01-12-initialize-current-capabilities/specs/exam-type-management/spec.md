## ADDED Requirements

### Requirement: 题型配置管理
系统应支持配置多种题型，每种题型有独立的参数（百分比、时间限制、整数位数、小数位数等）。

#### Scenario: 题型枚举定义
- **GIVEN** 系统启动
- **WHEN** 查看 ExamType 枚举
- **THEN** 包含五种类型：Addition、Subtraction、Multiplication、DivisionNoRemainder、DivisionWithRemainder

#### Scenario: 创建题型对象
- **GIVEN** 需要添加整数乘法题型，参数：百分比=25，时间=300秒，整数位数=3
- **WHEN** 创建 ExamObject 实例
- **THEN**: 成功创建题型对象，TotalTypeQty 增加 1

#### Scenario: 题型参数配置
- **GIVEN** 整数乘法题型对象
- **WHEN** 设置 Percent=25, TimeLimit=300, IntBits=3, DecBits=0
- **THEN** 对象属性正确存储这些值

#### Scenario: 计算各题型数量
- **GIVEN** 总题数=100，三种题型的百分比分别为 25、25、50
- **WHEN** 调用 UpdateExamQty()
- **THEN** 计算得到三种题型的数量分别为 25、25、50

#### Scenario: 题型统计追踪
- **GIVEN** 整数乘法题型，初始 TotalQty=25
- **WHEN** 用户答对 3 题，答错 1 题
- **THEN** CorrectQty=3, WrongQty=1, TotalQty=21（25-3-1）

#### Scenario: 题型时间统计
- **GIVEN** 整数乘法题型
- **WHEN** 用户完成该题型所有题目，总耗时 450 秒
- **THEN** ElapsedTime=450，可用于计算平均用时
