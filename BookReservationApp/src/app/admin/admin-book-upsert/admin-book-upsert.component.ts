import { Component, OnInit } from '@angular/core';
import { CategoriesService } from '../../_services/categories.service';
import { Category } from '../../_models/category';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { BooksService } from '../../_services/books.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from '../../_models/book';
import { PaginatedResponse } from '../../_models/paginated-response';
import { ApiResponse } from '../../_models/api-response';
import { FileUploader } from 'ng2-file-upload';

@Component({
  selector: 'app-admin-book-upsert',
  templateUrl: './admin-book-upsert.component.html',
  styleUrl: './admin-book-upsert.component.css'
})
export class AdminBookUpsertComponent implements OnInit {
  bookForm: FormGroup = new FormGroup({});
  uploader: FileUploader | undefined;
  isbn: string | null = null;
  validationErrors: string[] | undefined;
  categories: Category[] | undefined;

  constructor(
    private categoriesService: CategoriesService,
    private bookService: BooksService,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router) { }

  ngOnInit() {
    this.initializeForm();
    this.loadCategories();

    this.isbn = this.route.snapshot.params['isbn'];

    if (this.isbn) {
      this.loadBook(this.isbn);
    }

  }
  
  loadCategories() {
    this.categoriesService.getCategories().subscribe({
      next: (response: PaginatedResponse<Category[]>) => {
        if (response.result?.success) {
          this.categories = response.result.result;
        }
      },
      error: error => {
        this.toastr.error("An unexpected error occurred while retrieving the categories. Please try again later","Error");
      }
    });
  }

  initializeForm() {
    this.bookForm = this.formBuilder.group({
      id: [''],
      title: ['', Validators.required],
      description: ['', Validators.required],
      isbn: ['', [Validators.required, this.isbnValidator()]],
      author: ['', Validators.required],
      idCategory: ['', Validators.required],
      imageUrl: ['']
    });
  }

  isbnValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const isbn = control.value;
      const isValid = /^\d{13}$/.test(isbn);
      return isValid ? null : { invalidIsbn: true }
    }
  }

  loadBook(isbn: string) {
    this.bookService.getBook(isbn).subscribe({
      next: (book: Book) => {
        this.bookForm.patchValue(book);
      },
      error: () => {
        this.toastr.error("An error occurred while loading the book","Error");
      }
    })
  }

  submitForm() {
    console.log(this.bookForm.value);
    if (this.isbn) {
      this.updateBook();
    } else {
      this.createBook();
    }
  }

  updateBook() {
    const bookDto = this.bookForm.value;
    this.bookService.updateBook(bookDto, this.isbn!).subscribe({
      next: () => {
        this.toastr.success("Book updated successfully");
        this.router.navigateByUrl('admin/books/upsert/' + this.bookForm.controls['isbn'].value + '/image');
      },
      error: error => {
        this.validationErrors = error;
        this.toastr.error("An error occurred while updating the book", "Error");
      }
    });
  }

  createBook() {
    const bookDto = this.bookForm.value;
    this.bookService.createBook(bookDto).subscribe({
      next: response => {
        if (response.success) {
          this.toastr.success("Book created succesfully");
          this.router.navigateByUrl('admin/books/upsert/' + this.bookForm.controls['isbn'].value + '/image');
        }
      },
      error: (error: ApiResponse<any>) => {
        this.validationErrors = error.messages;
        this.toastr.error("An error ocurred while creating the book","Error");
      }
    })
  }

}
