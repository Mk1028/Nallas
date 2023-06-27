import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { JiraTasksComponent } from './jira-tasks/jira-tasks.component';
import { ModalComponent } from './modal/modal.component'
import { SharedService } from './shared.service';
import { AutocompleteService } from './autocomplete.service';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MdbFormsModule } from 'mdb-angular-ui-kit/forms';
import { MdbModalModule } from 'mdb-angular-ui-kit/modal';

@NgModule({
  declarations: [
    AppComponent,
    JiraTasksComponent,
    ModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MdbFormsModule,
    MdbModalModule
  ],
  providers: [SharedService, AutocompleteService],
  bootstrap: [AppComponent]
})
export class AppModule { }
