## ADDED Requirements

### Requirement: 文件服务抽象接口
(SHALL) 系统应提供统一的文件服务接口（IFileService），封装文件读写操作，支持日志记录、历史记录管理等。

#### Scenario: 写入日志内容
- **GIVEN** 日志文件路径和要写入的内容
- **WHEN** 调用 `IFileService.AppendLog("message")`
- **THEN** 内容追加到日志文件末尾，并包含时间戳

#### Scenario: 读取历史日志
- **GIVEN** 日志文件已存在多条记录
- **WHEN** 调用 `IFileService.ReadLogs()`
- **THEN** 返回所有日志内容的分行列表

#### Scenario: 备份配置文件
- **GIVEN** 配置文件已存在
- **WHEN** 调用 `IFileService.BackupConfig()`
- **THEN** 创建配置文件的备份（带时间戳）

#### Scenario: 文件不存在时创建
- **GIVEN** 目标文件不存在
- **WHEN** 调用写入方法
- **THEN** 自动创建文件并完成写入

### Requirement: 日志文件滚动策略
(SHALL) 系统应实现日志文件滚动机制，当日志文件过大时自动创建新文件。

#### Scenario: 日志文件超过大小限制
- **GIVEN** 当前日志文件大小超过 1MB
- **WHEN** 写入新的日志条目
- **THEN** 重命名当前日志文件（添加日期后缀），创建新的日志文件

#### Scenario: 保留最近日志文件
- **GIVEN** 日志目录中有多个历史日志文件
- **WHEN** 创建新日志文件时
- **THEN** 删除超过30天的旧日志文件

### Requirement: 历史记录管理
(SHALL) 系统应提供历史练习记录的管理功能，支持读取、查询和导出。

#### Scenario: 保存练习记录
- **GIVEN** 一次练习的完整数据（开始时间、题目列表、答题结果、用时等）
- **WHEN** 调用 `IFileService.SavePracticeRecord(record)`
- **THEN** 记录以结构化格式保存到历史文件

#### Scenario: 查询历史记录
- **GIVEN** 历史记录文件已存在多条记录
- **WHEN** 调用 `IFileService.GetPracticeHistory(dateFrom, dateTo)`
- **THEN** 返回指定日期范围内的练习记录列表

#### Scenario: 导出历史记录
- **GIVEN** 多条历史记录
- **WHEN** 调用 `IFileService.ExportToCsv()`
- **THEN** 生成 CSV 格式的导出文件
