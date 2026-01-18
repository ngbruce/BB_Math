# game-state Specification

## Purpose
TBD - created by archiving change refactor-code-structure-review. Update Purpose after archive.
## Requirements
### Requirement: 游戏状态管理器
(SHALL) 系统应提供游戏状态管理器（GameStateManager），负责管理游戏的全局状态，包括金币、暂停、进程监控等，替代原有的静态 bbMath 类。

#### Scenario: 初始化游戏状态
- **GIVEN** 新游戏开始
- **WHEN** 创建 GameStateManager 并调用 Initialize()
- **THEN** 加载配置，初始化金币、暂停次数等状态
- **THEN** examTtl 保持默认值（15），不在初始化时重置为0

#### Scenario: 答对题目获得金币
- **GIVEN** 当前金币数为 50，答对奖励为 3
- **WHEN** 答对一题后调用 `gameState.AwardCoins(3)`
- **THEN** 金币数更新为 53

#### Scenario: 使用辅助功能消耗金币（已废弃）
- **GIVEN** 当前金币数为 50，检查辅助花费为 1
- **WHEN** 使用检查辅助功能
- **THEN** 金币数减少为 49

#### Scenario: 金币不足时使用辅助功能（已废弃）
- **GIVEN** 当前金币数为 0
- **WHEN** 尝试使用需要消耗金币的辅助功能
- **THEN** 功能被禁用或提示金币不足

#### Scenario: 使用限次数暂停
- **GIVEN** 暂停类型为次数限制（pauseType=0），可用次数为 5
- **WHEN** 使用一次暂停
- **THEN** 可用次数减少为 4，计时器停止

#### Scenario: 使用限时间暂停
- **GIVEN** 暂停类型为时间限制（pauseType=1），剩余时间为 600 秒
- **WHEN** 暂停 60 秒后恢复
- **THEN** 剩余时间更新为 540 秒

#### Scenario: 进程黑名单监控
- **GIVEN** 黑名单包含 "Calculator" 和 "msedge"
- **WHEN** 检测到计算器或 Edge 浏览器进程启动
- **THEN** 自动结束这些进程（需要管理员权限）

#### Scenario: 错误惩罚机制
- **GIVEN** 错误惩罚设置为 2 题
- **WHEN** 答错一题
- **THEN** 剩余题目数增加 2 题

#### Scenario: 超时惩罚机制
- **GIVEN** 超时惩罚设置为 1 题，当前题超时
- **WHEN** 计时器归零
- **THEN** 剩余题目数增加 1 题

### Requirement: 状态持久化与恢复
(SHALL) 游戏状态管理器应支持状态的保存和恢复，确保应用程序重启后能够恢复之前的进度。

#### Scenario: 自动保存状态
- **GIVEN** 游戏进行中，状态发生变化（金币增减、暂停使用等）
- **WHEN** 状态变更后
- **THEN** 自动保存到配置文件（延迟保存，避免频繁IO）

#### Scenario: 恢复游戏状态
- **GIVEN** 应用程序重新启动
- **WHEN** 初始化 GameStateManager 时
- **THEN**: 从配置文件读取并恢复之前的游戏状态

#### Scenario: 完成练习后重置状态
- **GIVEN**: 一次练习已完成
- **WHEN**: 调用 `gameState.ResetForNewPractice()`
- **THEN**: 清除当前练习数据，保留金币等累积数据

