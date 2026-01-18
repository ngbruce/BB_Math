## Purpose
题目难度选择模块提供根据不同难度级别生成数学题目的能力,支持 Easy (简单)、Medium (中等)、Hard (困难) 三个级别,每个级别对应不同的数值范围和运算类型,满足不同数学能力水平的儿童学习需求。

## ADDED Requirements

### Requirement: 难度枚举定义
(SHALL) 系统应定义三个难度级别: Easy、Medium、Hard,分别对应不同的题目参数配置。

#### Scenario: 难度枚举存在
- **GIVEN** 系统启动
- **WHEN** 访问 Difficulty 枚举
- **THEN** 应包含三个枚举值: Easy、Medium、Hard

#### Scenario: 难度配置映射
- **GIVEN** Difficulty 枚举定义
- **WHEN** 查询难度配置
- **THEN** 每个难度应包含数值范围、运算类型、整数位数等参数

### Requirement: 难度参数配置
(SHALL) 系统应为每个难度级别定义明确的题目参数,包括数值范围、支持的运算类型等。难度定义如下:
- Easy (100以内加减法): 操作数和得数都是100以内
- Medium (100以内加减乘除): 操作数和得数都是100以内
- Hard (100以上加减乘除): 操作数和得数至少有一个在100以上

#### Scenario: Easy 难度配置
- **GIVEN** 选择 Easy 难度
- **WHEN** 生成题目
- **THEN** 仅支持加减法运算,操作数范围在10-99,得数必须<100

#### Scenario: Medium 难度配置
- **GIVEN** 选择 Medium 难度
- **WHEN** 生成题目
- **THEN** 支持加减乘除四则运算,操作数范围在10-99,得数必须<100

#### Scenario: Hard 难度配置
- **GIVEN** 选择 Hard 难度
- **WHEN** 生成题目
- **THEN** 支持加减乘除四则运算,操作数范围在100-999,至少有一个操作数或得数>=100

### Requirement: 难度选择UI交互
(SHALL) 系统应通过下拉菜单控件 (cbDifficultySelect) 提供难度选择功能,支持用户在答题开始前选择难度。

#### Scenario: 显示难度选项
- **GIVEN** 主窗体加载完成
- **WHEN** 查看 cbDifficultySelect 控件
- **THEN** 应显示三个选项: "100以内加减法"、"100以内加减乘除"、"100以上加减乘除"

#### Scenario: 默认难度选择
- **GIVEN** 主窗体首次加载
- **WHEN** 查看当前选中的难度
- **THEN** 默认应选中 "100以内加减法" (Easy)

#### Scenario: 切换难度
- **GIVEN** 用户已选择某个难度
- **WHEN** 用户从下拉菜单选择另一个难度
- **THEN** 系统应更新当前难度状态,下次答题使用新难度

### Requirement: 难度与题目生成集成
(SHALL) 系统应根据当前选择的难度生成题目,确保题目参数符合难度配置要求。出题逻辑应先选择100以内随机数,然后根据难度要求调整。

#### Scenario: Easy 难度生成加减法题目
- **GIVEN** 当前难度为 Easy
- **WHEN** 生成加法或减法题目
- **THEN** 操作数应在10-99范围,且得数必须<100;如果得数>=100则重新生成

#### Scenario: Medium 难度生成四则运算题目
- **GIVEN** 当前难度为 Medium
- **WHEN** 生成加减乘除题目
- **THEN** 操作数应在10-99范围,且得数必须<100;如果得数>=100则重新生成

#### Scenario: Hard 难度生成四则运算题目
- **GIVEN** 当前难度为 Hard
- **WHEN** 生成加减乘除题目
- **THEN** 至少有一个操作数或得数>=100;如果所有数都<100则重新生成

#### Scenario: Hard 难度除法题目保证被除数>=100
- **GIVEN** 当前难度为 Hard
- **WHEN** 生成除法题目(无余数或有余数)
- **THEN** 被除数应>=100;如果被除数<100则重新生成

### Requirement: 练习记录包含难度信息
(SHALL) 系统应在练习记录中保存难度信息,包括在 CSV 导出文件和日志文件中。

#### Scenario: PracticeRecord 包含难度字段
- **GIVEN** 完成一次练习
- **WHEN** 查看 PracticeRecord 对象
- **THEN** 应包含 Difficulty 属性,记录本次练习使用的难度

#### Scenario: CSV 文件包含难度信息
- **GIVEN** 导出练习记录为 CSV 格式
- **WHEN** 查看 CSV 文件内容
- **THEN** 第一行标题应包含 "难度" 列,数据行应记录具体的难度级别

#### Scenario: 日志文件包含难度信息
- **GIVEN** 答题完成并保存日志
- **WHEN** 查看 bbmath.dat 日志文件
- **THEN** 应包含本次练习的难度信息
