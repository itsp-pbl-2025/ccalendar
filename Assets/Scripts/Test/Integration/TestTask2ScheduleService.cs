using System;
using System.Collections.Generic;
using System.Linq;
using AppCore.UseCases;
using Domain.Entity;
using Infrastructure.Data.DAO;
using Test.MockData;
using NUnit.Framework;

namespace Test.Integration
{
    public class TestTask2ScheduleService
    {
        [Test]
        public void GenerateSchedule()
        {
            // テスト用コンテキストを取得
            var ctx = InTestContext.Context;
            
            // 1. テスト用のタスクを用意
            var tasks = MockTask.GetMockTasksWithoutId().Select(task => task.ToDomain()).ToList();
            
            // 2. 開始日時を設定
            var startDate = new DateTime(2025, 7, 1, 9, 0, 0);

            // 3. サービス実行
            var service = ctx.GetService<Task2ScheduleService>();
            service.GenerateSchedule(tasks, startDate);

            // 4. 生成されたスケジュールを取得
            var schedules = ctx
                .GetService<ScheduleService>()
                .GetSchedules()
                .AsValueEnumerable()
                .ToDictionary(x => x.Title);

            // 5. 期待順序(締切順)で検証
            var sortedTasks = tasks.OrderBy(t => t.Deadline).ToList();
            var current = startDate;
            foreach (var task in sortedTasks)
            {
                Assert.IsTrue(schedules.TryGetValue(task.Title, out var sched), 
                    $"スケジュール '{task.Title}' が見つかりません。");
                
                // Descriptionのチェック
                Assert.AreEqual(task.Description, sched.Description, 
                    $"'{task.Title}' の説明が一致しません。");
                
                // Durationの開始・終了時刻チェック
                Assert.AreEqual(current, sched.Duration.StartTime, 
                    $"'{task.Title}' の開始時刻が期待値と違います。");
                Assert.AreEqual(current.Add(task.Duration), sched.Duration.EndTime, 
                    $"'{task.Title}' の終了時刻が期待値と違います。");
                
                schedules.Remove(task.Title);
                current = current.Add(task.Duration);
            }

            // 6. 余計なスケジュールがないこと
            Assert.IsTrue(schedules.Count == 0, "想定外のスケジュールが残っています。");

            // 7. 後片付け
            ctx.Dispose();
        }
    }
}
