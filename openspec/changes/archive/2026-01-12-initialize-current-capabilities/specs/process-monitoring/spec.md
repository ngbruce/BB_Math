## ADDED Requirements

### Requirement: 进程监控与黑名单管理
系统应监控系统进程，自动结束黑名单中的进程，防止用户使用计算器等工具作弊。

#### Scenario: 检测到计算器进程
- **GIVEN** 进程黑名单包含 "Calculator" 和 "calc"
- **WHEN** 用户启动 Windows 计算器
- **THEN** 系统检测到进程并自动结束它

#### Scenario: 检测到浏览器进程
- **GIVEN** 进程黑名单包含 "msedge", "SogouExplorer", "360se"
- **WHEN** 用户启动 Edge 浏览器
- **THEN** 系统检测到进程并自动结束它

#### Scenario: 监控频率
- **GIVEN** 系统每 1000ms 检查一次进程
- **WHEN** 黑名单进程启动
- **THEN** 在 1 秒内检测到并结束进程

#### Scenario: 无管理员权限时
- **GIVEN** 应用程序没有管理员权限
- **WHEN** 尝试结束系统进程
- **THEN** 操作失败，记录警告日志

#### Scenario: 进程结束失败处理
- **GIVEN** 尝试结束进程时发生异常
- **WHEN** 捕获到异常
- **THEN** 记录错误信息到调试输出，继续监控其他进程
