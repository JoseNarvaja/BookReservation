import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Book } from '../../_models/book';
import { BooksService } from '../../_services/books.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../environment/environment';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent implements OnInit {
  uploader: FileUploader | undefined;
  isbn: string | null = null;
  book: Book | null = null
  baseUrl: string = environment.apiUrl;
  hasBaseDropZoneOver = false;

  constructor(private bookService: BooksService,
    private router: Router,
    private toastr: ToastrService,
    private route: ActivatedRoute,) { }

  ngOnInit() {
    this.isbn = this.route.snapshot.params['isbn'];
    if (this.isbn) {
      this.bookService.getBook(this.isbn).subscribe({
        next: (response: Book) => {
          this.book = response;
        }
      });
      this.initializeUploader();
    }
  }

  initializeUploader() {
    const token = localStorage.getItem('token');

    if (this.isbn && token) {
      this.uploader = new FileUploader({
        url: this.baseUrl + '/books/' + this.isbn + '/image',
        authToken: 'Bearer ' + token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 10 * 1024 * 1024
      });

      this.uploader.onAfterAddingFile = (file) => {
        file.withCredentials = false;
      };

      this.uploader.onSuccessItem = (item, response, status, headers) => {
        this.toastr.success('Your image was uploaded successfuly', 'Image uploaded');
        this.router.navigateByUrl('admin/books');
      }

      this.uploader.onErrorItem = (item, response, status, headers) => {
        this.toastr.error('An error ocurred while updating the book. Please try later', 'Error');
        this.router.navigateByUrl('admin/books');
      }

      this.uploader.onAfterAddingFile = (item) => {
        item.method = "PUT";
      }
    }
  }

  clearQueue() {
    this.uploader?.clearQueue();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }
}
