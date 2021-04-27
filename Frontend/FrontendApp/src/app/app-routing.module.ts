import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterLostDogComponent } from './Components/register-lost-dog/register-lost-dog.component';
import { HomePageComponent } from './Components/home-page/home-page.component';
import { LoginComponent } from './Components/login/login.component';
import { RegisterUserComponent } from './Components/register-user/register-user.component';
import { EditLostDogComponent } from './Components/edit-lost-dog/edit-lost-dog.component';
import { EditContactInfoComponent } from './Components/edit-contact-info/edit-contact-info.component';
import { FilterLostDogsComponent } from './Components/filter-lost-dogs/filter-lost-dogs.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'register', component: RegisterUserComponent },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomePageComponent },
  { path: 'register-lost-dog', component: RegisterLostDogComponent },
  { path: 'edit-lost-dog/:dogId', component: EditLostDogComponent },
  { path: 'edit-contact-info', component: EditContactInfoComponent },
  { path: 'search', component: FilterLostDogsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
