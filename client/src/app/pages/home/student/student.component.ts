import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { StudentService } from './student.service';


@Component({
  selector: 'app-student',
  templateUrl: './student.component.html',
  styleUrl: './student.component.css'
})
export class StudentComponent {
  dataSource: any[] = [

  ];

  displayedColumns: string[] = ['name', 'email', "age", 'action'];
  form!: FormGroup;
  editDataId!: string;

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private studentService: StudentService) {

    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', Validators.required],
      age: ['', Validators.required],
    });
  }

  ngOnInit() {
this.getStudent()
  }

  getStudent() {
    this.studentService.getStudent().subscribe(res => {
      console.log(res);
      this.dataSource = res.items;
    },
      err => {
        console.log(err);
      }
    )
  }

  submitForm() {
    if (this.form.valid) {
      if (this.editDataId) {
        this.studentService.putStudent(this.editDataId,this.form.value).subscribe(res => {
          console.log(res);
          this.getStudent();
        },
          err => {
            console.log(err);
          }
        )
      }
      else {
        this.studentService.postStudent(this.form.value).subscribe(res => {
          console.log(res);
          this.getStudent();
        },
          err => {
            console.log(err);
          }
        )
        console.log(this.form.value);
      }
    }
  }
  editItem(data: any) {

    this.editDataId = data.id;

    this.form.patchValue(data);
  }
  deleteItem(id: any) {
    console.log(id);
    this.studentService.deleteStudent(id).subscribe(res => {
      console.log(res);
      this.getStudent()
    },
      err => {
        console.log(err);
      }
    )
  }

  logout() {

  }

}
