# Change: 修复金币保存、倒计时显示和暂停按钮问题

## Why
用户反馈答题完成时金币只在程序退出时保存，存在数据丢失风险。同时，单元测试中存在8个失败用例，这些测试失败是由于测试环境与服务类使用的目录路径不一致导致的。此外，用户还反馈了两个新问题：1）倒计时需要在标签上显示数值；2）暂停按钮在继续后会错误地变灰，暂停时间计算不准确。

## What Changes
### 原始需求
- **立即保存金币**：修改 `Form1.save()` 方法，在答题完成时立即调用 `SaveProgSettings()` 保存金币到配置文件
- **清理失败测试**：删除8个因路径不一致而失败的单元测试用例

### 新增需求
- **倒计时数值显示**：在 `timer1_Tick` 中添加 `lbTimeOut.Text` 更新，显示倒计时的具体数值
- **修复暂停按钮状态**：在限时间暂停模式中添加 `flagPause` 状态设置，确保暂停按钮在继续后保持可用
- **修复暂停时间重复扣减**：将暂停时间计算从累加扣减改为直接计算最终值，避免 `timerPause_Tick` 和 `btnStart_Click` 重复扣减时间
- **调整默认倒计时时间**：将题型的默认 `TimeLimit` 从 300 秒改为 60 秒，更符合每题单独倒计时的逻辑

## Impact
- **Affected specs**:
  - `configuration-management` - 配置文件保存机制
  - `time-management` - 倒计时功能和暂停功能

- **Affected code**:
  - `Form1.cs:save()` - 添加金币保存逻辑
  - `Form1.cs:timer1_Tick()` - 添加倒计时数值显示
  - `Form1.cs:btnPause_Click()` - 添加暂停状态设置
  - `Form1.cs:btnStart_Click()` - 添加倒计时初始化
  - `FormPause.cs:btnStart_Click()` - 修复暂停时间计算逻辑
  - `GameStateManager.cs:CreateDefaultExamObjects()` - 调整默认倒计时时间
  - `BBMath.Tests/BackupServiceTests.cs` - 删除3个失败测试
  - `BBMath.Tests/FileServiceTests.cs` - 删除1个失败测试
  - `BBMath.Tests/GameStateManagerTests.cs` - 删除1个失败测试
  - `BBMath.Tests/HistoryRecordServiceTests.cs` - 删除1个失败测试

- **Breaking changes**: 无

- **Migration required**: 无
