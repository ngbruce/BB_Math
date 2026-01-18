## MODIFIED Requirements

### Requirement: 历史记录管理
(SHALL) 系统应提供历史练习记录的管理功能，支持读取、查询和导出。历史记录应包含难度信息,用于区分不同难度级别的练习成绩。

#### Scenario: 保存练习记录
- **GIVEN** 一次练习的完整数据（开始时间、题目列表、答题结果、用时等）
- **WHEN** 调用 `IFileService.SavePracticeRecord(record)`
- **THEN** 记录以结构化格式保存到历史文件,包含难度级别字段

#### Scenario: 查询历史记录
- **GIVEN** 历史记录文件已存在多条记录
- **WHEN** 调用 `IFileService.GetPracticeHistory(dateFrom, dateTo)`
- **THEN** 返回指定日期范围内的练习记录列表,每条记录包含难度信息

#### Scenario: 导出历史记录
- **GIVEN** 多条历史记录
- **WHEN** 调用 `IFileService.ExportToCsv()`
- **THEN** 生成 CSV 格式的导出文件,包含难度列

#### Scenario: CSV 文件包含难度列
- **GIVEN** PracticeRecord 对象包含 Difficulty 属性
- **WHEN** 调用 `ToCsvString()` 方法
- **THEN** 返回的 CSV 字符串第一行应包含 "难度" 列标题,数据行应包含对应的难度值 (Easy/Medium/Hard)

#### Scenario: 日志文件包含难度信息
- **GIVEN** 完成一次练习并调用 save 方法
- **WHEN** 写入日志文件
- **THEN** 日志内容应包含本次练习的难度级别信息
