import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { tap } from 'rxjs/operators';
import { Message } from 'src/app/_models/message';
import { AlertyfyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { MessageService } from 'src/app/_services/message.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-memeber-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  messageContent: string;

  constructor(public messageService: MessageService,
    private alertify: AlertyfyService) { }

  ngOnInit() {}

  sendMessage() {
    this.messageService.sendMessage(this.username,
      this.messageContent)
      .then(() => {
        // this.messages.push(message);
        this.messageForm.reset();
        // this.messages.unshift(message);
        // this.newMessage.content = '';
      }, error => {
        this.alertify.error(error);
      });
  }
}
