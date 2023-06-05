import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

enum JiraStatuses {
  ToDo = 1,
  InProgress = 2,
  CodeReview = 3,
  Testing = 4,
  Done = 5
}

enum Assignees {
  A1 = 1,
  A2 = 2
}

interface JiraTask {
  id: number;
  name: string;
  status: JiraStatuses;
  assignedTo: Assignees;
}

@Component({
  selector: 'app-jira-tasks',
  templateUrl: './jira-tasks.component.html',
  styleUrls: ['./jira-tasks.component.css']
})
export class JiraTasksComponent implements OnInit {
  jiraTasks: JiraTask[] = [];
  newTask: JiraTask = {
    id: 0,
    name: '',
    status: JiraStatuses.ToDo,
    assignedTo: Assignees.A1
  };

  updateTask: JiraTask = { id: 0, name: '', status: JiraStatuses.ToDo, assignedTo: Assignees.A1 };
  showUpdateForm = false;

  jiraStatuses = JiraStatuses;
  assignees = Assignees; 

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadJiraTasks();
  }

  loadJiraTasks(): void {
    this.http.get<JiraTask[]>('https://localhost:7218/jiratasks').subscribe(tasks => {
      this.jiraTasks = tasks;
    });
  }

  createTask(): void {
    console.log('New Task:', this.newTask);
    this.http.post<JiraTask>('https://localhost:7218/jiratasks', this.newTask).subscribe(response => {
      this.jiraTasks.push(response);
      this.newTask = {
        id: 0,
        name: '',
        status: JiraStatuses.ToDo,
        assignedTo: Assignees.A1
      };
    });
  }

  openUpdateForm(task: JiraTask): void {
    this.updateTask = { ...task }; // Clone the task object
    this.showUpdateForm = true;
  }

  updateTaskEntry(): void {
    console.log('Updated Task:', this.updateTask);
    this.http.put<void>(`https://localhost:7218/jiratasks/${this.updateTask.id}`, this.updateTask).subscribe(() => {
      const index = this.jiraTasks.findIndex(t => t.id === this.updateTask.id);
      if (index !== -1) {
        this.jiraTasks[index] = { ...this.updateTask };
      }

      this.cancelUpdate(); // Hide the update form
    });
  }

  cancelUpdate(): void {
    this.updateTask = { id: 0, name: '', status: JiraStatuses.ToDo, assignedTo: Assignees.A1 };
    this.showUpdateForm = false;
  }

  deleteTask(task: JiraTask): void {
    this.http.delete<void>(`https://localhost:7218/jiratasks/${task.id}`).subscribe(() => {
      this.jiraTasks = this.jiraTasks.filter(t => t.id !== task.id);
    });
  }
}
