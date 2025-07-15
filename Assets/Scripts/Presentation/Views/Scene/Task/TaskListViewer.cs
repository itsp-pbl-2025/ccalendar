using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using AppCore.UseCases;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Utilities;

namespace Presentation.Views.Scene.Task
{
    public class TaskListViewer : MonoBehaviour
    {
        [SerializeField] private RectTransform taskListParent;
        [SerializeField] private GameObject taskItemPrefab;
        [SerializeField] private float itemSpacing = 20f;

        private TaskService _taskService;
        private readonly List<GameObject> _taskItems = new();

        private void Start()
        {
            _taskService = InAppContext.Context.GetService<TaskService>();

            // タスクリスト更新イベント登録
            InAppContext.EventDispatcher.AddGlobalEventListener(this, GlobalEvent.OnTaskCreated, _ => RefreshTaskList());

            RefreshTaskList();
        }

        public void RefreshTaskList()
        {
            ClearTaskList();
            DisplayTasks();
        }

        private void DisplayTasks()
        {
            var tasks = _taskService.GetTask();

            foreach (var task in tasks)
            {
                CreateTaskItem(task);
            }
        }

        private void CreateTaskItem(CCTask task)
        {
            if (taskItemPrefab == null || taskListParent == null) return;

            var taskItem = Instantiate(taskItemPrefab, taskListParent);
            _taskItems.Add(taskItem);

            SetTaskItemData(taskItem, task);
        }

        private void SetTaskItemData(GameObject taskItem, CCTask task)
        {
            var titleText = taskItem.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            var descriptionText = taskItem.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            var priorityText = taskItem.transform.Find("PriorityText")?.GetComponent<TextMeshProUGUI>();
            var deadlineText = taskItem.transform.Find("DeadlineText")?.GetComponent<TextMeshProUGUI>();

            if (titleText != null) titleText.text = task.Title;
            if (descriptionText != null) descriptionText.text = task.Description;
            if (priorityText != null) priorityText.text = $"優先度: {task.Priority}";
            if (deadlineText != null) deadlineText.text = $"締め切り: {task.Deadline.ToString("yyyy年MM月dd日 HH:mm")}";
        }

        private void ClearTaskList()
        {
            foreach (var item in _taskItems)
            {
                if (item != null)
                {
                    DestroyImmediate(item);
                }
            }

            _taskItems.Clear();
        }
    }
}