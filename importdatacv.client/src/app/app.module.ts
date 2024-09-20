import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { DataDisplayComponent } from './data-display/data-display.component';
import { MessengerMessageComponent } from './meta/messenger-message.component';




@NgModule({
  declarations: [
    AppComponent,
    DataDisplayComponent,
    MessengerMessageComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
