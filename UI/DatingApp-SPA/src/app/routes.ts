import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListesComponent } from './listes/listes.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolver/member-details';
import { MemberListResolver } from './_resolver/member-list';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolver/member-edit';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,
            resolve: {users: MemberListResolver} },
            { path: 'members/:id', component: MemberDetailComponent,
            resolve: {user: MemberDetailResolver} },
            { path: 'member/edit', component: MemberEditComponent,
            resolve: { user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges] },
            { path: 'messages', component: MessagesComponent },
            { path: 'list', component: ListesComponent }
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
