import { Component, OnInit} from '@angular/core';
import { MdbModalRef } from 'mdb-angular-ui-kit/modal';
import { HttpClient } from '@angular/common/http';
import { JiraTask, JiraStatuses, Assignees, SharedService } from '../shared.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})
export class ModalComponent implements OnInit {
  newTask: JiraTask = {
    id: 0,
    name: '',
    status: JiraStatuses.ToDo,
    assignedTo: Assignees.A1
  };
  jiraStatuses = JiraStatuses;
  assignees = Assignees;
  jiraTasks: JiraTask[] = [];
  newTaskForm!: FormGroup;

  constructor(public modalRef: MdbModalRef<ModalComponent>, private http: HttpClient, private sharedService: SharedService, private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.sharedService.jiraTasks$.subscribe(tasks => {
      this.jiraTasks = tasks;
    });
    this.initializeForm();
  }
  
  initializeForm(): void {
    this.newTaskForm = this.formBuilder.group({
      name: ['', Validators.required],
      status: [null, Validators.required],
      assignedTo: [null, Validators.required]
    });
  }

  createTask(): void {
    if (this.newTaskForm.valid) {
      // Form is valid, proceed with task creation
      this.newTask['name'] = this.newTaskForm.controls['name'].value;
      this.newTask['status'] = this.newTaskForm.controls['status'].value;
      this.newTask['assignedTo'] = this.newTaskForm.controls['assignedTo'].value;
      console.log('New Task:', this.newTask);
      this.http
        .post<JiraTask>('https://localhost:7218/jiratasks', this.newTask)
        .subscribe(response => {
          this.jiraTasks.push(response);
        });
      this.modalRef.close();
    } else {
      // Form is invalid, display validation messages
      this.newTaskForm.markAllAsTouched();
    }
  }
}
