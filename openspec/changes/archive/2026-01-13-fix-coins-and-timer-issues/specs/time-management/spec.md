## MODIFIED Requirements
### Requirement: 时间限制与超时管理
系统 SHALL 为每道题目设置时间限制，超时自动判定为错误并增加惩罚题目。系统 SHALL 在开始答题时启动倒计时，实时显示剩余时间。系统 SHALL 确保倒计时定时器在题目生成时正确启动。

#### Scenario: 题目时间限制
- **GIVEN** 整数乘法题型配置 TimeLimit=60 秒
- **WHEN** 生成该题型题目
- **THEN** 倒计时器初始化为 60 秒

#### Scenario: 倒计时正常进行
- **GIVEN** 当前题目剩余时间 150 秒
- **WHEN** 经过 1 秒
- **THEN** 剩余时间更新为 149 秒

#### Scenario: 在时间限制内答题
- **GIVEN** 题目剩余时间 60 秒
- **WHEN** 用户提交正确答案
- **THEN** 记录用时 = 初始时间 - 剩余时间，进入下一题

#### Scenario: 超时惩罚
- **GIVEN** 题目剩余时间 0 秒，超时惩罚 punishmentTimeOut=1
- **WHEN** 超时发生时
- **THEN** 剩余题目数增加 1 题，生成新题目，重置倒计时器

#### Scenario: 超时进度条显示
- **GIVEN** 题目时间限制 120 秒，当前剩余 60 秒
- **WHEN** 显示进度条
- **THEN** 进度条显示 50% 进度

#### Scenario: 时间限制随题型变化
- **GIVEN** 整数乘法题型 TimeLimit=60 秒，有余数除法 TimeLimit=60 秒
- **WHEN** 生成不同题型题目
- **THEN** 倒计时器使用对应题型的 TimeLimit 值

#### Scenario: 开始答题时启动倒计时
- **GIVEN** 用户点击"开始"按钮
- **WHEN** 系统生成第一道题目
- **THEN** 应启动倒计时定时器
- **THEN** 倒计时进度条应开始从最大值递减
- **THEN** 界面应实时显示剩余时间数值

#### Scenario: 答题时倒计时持续运行
- **GIVEN** 倒计时已启动
- **WHEN** 用户答题（提交答案）
- **THEN** 倒计时应继续运行
- **THEN** 不应因答题操作而停止或重置

#### Scenario: 倒计时超时
- **GIVEN** 倒计时剩余时间达到 0
- **WHEN** 倒计时超时
- **THEN** 应触发超时处理逻辑
- **THEN** 应应用超时惩罚（如增加题目）
- **THEN** 应记录超时事件

#### Scenario: Presenter 生成题目后启动计时器
- **GIVEN** Presenter 生成题目
- **WHEN** 题目显示在界面上
- **THEN** 应启动 timer1 定时器
- **THEN** 应设置正确的超时时间
- **THEN** 应更新进度条显示
- **THEN** 应更新倒计时数值显示

#### Scenario: MVP 重构后计时器绑定
- **GIVEN** 系统已完成 MVP 模式重构
- **WHEN** 使用 Presenter 生成题目
- **THEN** 计时器控制逻辑应保持正确
- **THEN** 不应因重构而丢失计时器启动逻辑
