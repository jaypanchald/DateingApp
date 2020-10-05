import { User } from '../../_models/user';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertyfyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [
    { value: 'male', Display: 'Male' },
    { value: 'female', Display: 'Female' }
  ];
  userPrams: any = {};
  pagination: Pagination;

  constructor(private userService: UserService,
    private alertify: AlertyfyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.userPrams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userPrams.minAge = 18;
    this.userPrams.maxAge = 99;
    this.userPrams.orderBy = 'lastActive';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

resetFilter() {
  this.userPrams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userPrams.minAge = 18;
    this.userPrams.maxAge = 99;
    this.loadUsers();
}

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage
      , this.userPrams)
      .subscribe(
        (res: PaginatedResult<User[]>) => {
          this.users = res.result;
          this.pagination = res.pagination;
        }, error => {
          this.alertify.error(error);
        }
      );
  }
}
