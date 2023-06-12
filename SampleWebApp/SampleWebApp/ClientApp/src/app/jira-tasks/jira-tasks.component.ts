import { Component, OnInit, ViewChild, ElementRef} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JiraTask, JiraStatuses, Assignees, SharedService} from '../shared.service';
import { ModalComponent } from '../modal/modal.component';
import { MdbModalRef, MdbModalService } from 'mdb-angular-ui-kit/modal';

@Component({
  selector: 'app-jira-tasks',
  templateUrl: './jira-tasks.component.html',
  styleUrls: ['./jira-tasks.component.css']
})
export class JiraTasksComponent implements OnInit {
  @ViewChild('nameInput') nameInput!: ElementRef;

  updateTask: JiraTask = {
    id: 0,
    name: '',
    status: JiraStatuses.ToDo,
    assignedTo: Assignees.A1
  };
  showUpdateForm = false;
  sideNavOpen = false;
  jiraStatuses = JiraStatuses;
  assignees = Assignees;
  jiraTasks: JiraTask[] = [];
  selectedJira: JiraTask | null = null;

  constructor(private http: HttpClient, private modalService: MdbModalService, private sharedService: SharedService) { }
  
  ngOnInit(): void {
    this.sharedService.loadJiraTasks();
    this.sharedService.jiraTasks$.subscribe(tasks => {
      this.jiraTasks = tasks;
      if (this.jiraTasks.length >= 1) {
        this.selectJira(this.jiraTasks[0]);
      }
    });
  }
  
  openUpdateForm(task: JiraTask): void {
    this.updateTask = { ...task }; // Clone the task object
    this.showUpdateForm = true;
  }

  updateTaskEntry(): void {
    console.log('Updated Task:', this.updateTask);
    this.http
      .put<void>(
        `https://localhost:7218/jiratasks/${this.updateTask.id}`,
        this.updateTask
      )
      .subscribe(() => {
        const index = this.jiraTasks.findIndex(t => t.id === this.updateTask.id);
        if (index !== -1) {
          this.jiraTasks[index] = { ...this.updateTask };
        }
      });
  }

  deleteTask(task: JiraTask): void {
    this.http
      .delete<void>(`https://localhost:7218/jiratasks/${task.id}`)
      .subscribe(() => {
        this.jiraTasks = this.jiraTasks.filter(t => t.id !== task.id);
      });
  }

  modalRef: MdbModalRef<ModalComponent> | null = null;

  openModal() {
    this.modalRef = this.modalService.open(ModalComponent, {
      modalClass: 'modal-dialog-centered'
    })
  }

  selectJira(jira: JiraTask): void {
    this.selectedJira = jira;
  }

  moveCursorToEnd(inputElement: HTMLInputElement): void {
    const length = inputElement.value.length;
    inputElement.setSelectionRange(length, length);
  }

  editMode = false;

  toggleEditMode(): void {
    this.editMode = !this.editMode;
    this.nameInput.nativeElement.focus();
    this.moveCursorToEnd(this.nameInput.nativeElement);
  }

  updateJiraName(newName: string): void {
    if (this.selectedJira && this.selectedJira.name !== null) {
      this.selectedJira.name = newName.trim();
      this.updateTask = this.selectedJira;
      this.updateTaskEntry();
      this.toggleEditMode();
    }
  }

  updateJiraStatusOrAssignee(): void {
    if (this.selectedJira) {
      this.updateTask = this.selectedJira;
      this.updateTaskEntry();
    }
  }
}

