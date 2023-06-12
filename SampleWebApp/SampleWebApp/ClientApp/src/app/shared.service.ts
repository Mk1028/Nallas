import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

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

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private jiraTasksSubject = new BehaviorSubject<JiraTask[]>([]);
  jiraTasks$ = this.jiraTasksSubject.asObservable();

  constructor(private http: HttpClient) { }

  loadJiraTasks(): void {
    this.http
      .get<JiraTask[]>('https://localhost:7218/jiratasks')
      .subscribe(tasks => {
        this.jiraTasksSubject.next(tasks);
      });
  }
}
