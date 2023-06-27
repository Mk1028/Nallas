import { Component, OnInit} from '@angular/core';
import { MdbModalRef } from 'mdb-angular-ui-kit/modal';
import { HttpClient } from '@angular/common/http';
import { JiraTask, JiraStatuses, Assignees, SharedService } from '../shared.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Guid } from "guid-typescript";
import { AutocompleteService } from '../autocomplete.service';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})

export class ModalComponent implements OnInit {
  newTask: JiraTask = {
    id: Guid.createEmpty().toString(),
    name: '',
    status: JiraStatuses.ToDo,
    assignedTo: Assignees.Person1
  };
  jiraStatuses = JiraStatuses;
  assignees = Assignees;
  jiraTasks: JiraTask[] = [];
  newTaskForm!: FormGroup;
  suggestions: any[] = [];

  constructor(public modalRef: MdbModalRef<ModalComponent>, private http: HttpClient, private sharedService: SharedService, private formBuilder: FormBuilder, private autocompleteService: AutocompleteService) { }

  ngOnInit(): void {
    this.sharedService.jiraTasks$.subscribe(tasks => {
      this.jiraTasks = tasks;
    });
    this.initializeForm();
  }

  initializeForm(): void {
    this.newTaskForm = this.formBuilder.group({
      id: Guid.createEmpty(),
      name: ['', Validators.required],
      status: [null, Validators.required],
      assignedTo: [null, Validators.required]
    });
  }

  createTask(): void {
    if (this.newTaskForm.valid) {
      // Form is valid, proceed with task creation
      this.newTask['id'] = Guid.create().toString();
      this.newTask['name'] = this.newTaskForm.controls['name'].value;
      this.newTask['status'] = this.newTaskForm.controls['status'].value;
      this.newTask['assignedTo'] = this.newTaskForm.controls['assignedTo'].value;
      console.log('New Task:', this.newTask);
      this.http
          .post<JiraTask>('https://localhost:7218/api/JiraTasks', this.newTask)
          .subscribe(response => {
            this.jiraTasks.push(response);
          });
      this.modalRef.close();
    } else {
      // Form is invalid, display validation messages
      this.newTaskForm.markAllAsTouched();
    }
  }

  selectSuggestion(suggestion: any): void {
    console.log(suggestion);
  }

  addressAutoComplete(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const query = inputElement.value;

    this.autocompleteService.getAutocompleteSuggestions(query)
      .subscribe((response: any) => {
        this.suggestions = response.addresses || response.places || response.pois || [];
      });
  }
}
