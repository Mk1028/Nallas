import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { JiraTasksComponent } from './jira-tasks/jira-tasks.component';

const routes: Routes = [
  { path: '', component: JiraTasksComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
