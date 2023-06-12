import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export enum JiraStatuses {
  ToDo = 1,
  InProgress = 2,
  CodeReview = 3,
  Testing = 4,
  Done = 5
}

export enum Assignees {
  A1 = 1,
  A2 = 2
}

export interface JiraTask {
  id: number;
  name: string;
  status: JiraStatuses;
  assignedTo: Assignees;
}

@Component({
  selector: 'app-common-modules',
  templateUrl: './common-modules.component.html',
  styleUrls: ['./common-modules.component.css']
})
export class CommonModulesComponent {
  
  jiraTasks: JiraTask[] = [];

  constructor(private http: HttpClient) { }

  loadJiraTasks(): void {
    this.http
      .get<JiraTask[]>('https://localhost:7218/jiratasks')
      .subscribe(tasks => {
        this.jiraTasks = tasks;
      });
    console.log('New Task:', this.jiraTasks);
  }
}
