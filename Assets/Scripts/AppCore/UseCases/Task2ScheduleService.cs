using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using System;
using System.Linq;

namespace AppCore.UseCases
{
    public class Task2ScheduleService : IService
    {
        
        public string Name { get; }
        public void Setup()
        {
            
        }

        private readonly IContext _context;
        private readonly ScheduleService _scheduleService;
        
        public Task2ScheduleService(IContext context, string name = "")
        {
            Name = !string.IsNullOrEmpty(name) ? name : GetType().Name;
            _context = context;
            _scheduleService = context.GetService<ScheduleService>();
        }
        
        // public void Setup() {}
        
        /// <summary>
        /// スケジュールを自動生成するためのメソッド。
        /// TODO: すでに存在するスケジュールと重複しないようにする。
        /// TODO: これまでのタスクの実行履歴を考慮したスケジュールの自動生成を行う。
        /// 
        /// </summary>
        /// <param name="tasks">スケジュールを設定するタスクのリスト</param>
        /// <param name="startDate">スケジュールの自動設定を開始する開始時刻</param>
        public void GenerateSchedule(IEnumerable<CCTask> tasks, CCDateTime startDate)
        {
            List<CCTask> sortedTasks = SortCCTasksByDeadline(tasks);

            CCDateTime currentDate = startDate;
            foreach (var task in sortedTasks)
            {   
                // タスクの開始日時を設定
                ScheduleDuration duration = new ScheduleDuration(
                    currentDate, currentDate.Add(task.Duration)
                );
                Schedule schedule = new Schedule(
                    0, // Idは後で設定されるため0を仮置き
                    task.Title,
                    task.Description,
                    duration
                );
                _scheduleService.CreateSchedule(schedule);
                
                currentDate = currentDate.Add(task.Duration);
            }
            
        }
        /// <summary>
        /// CCTaskを任意の方法でソートする。
        /// 実装は具体的な要件に依存するため、ひとまずDeadlineの近さを考慮してソートする。
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        private List<CCTask> SortCCTasksByDeadline(IEnumerable<CCTask> tasks)
        {
            return tasks.OrderBy(task => task.Deadline).ToList();
        }
        
        public void Dispose()
        {
        }
    }
}