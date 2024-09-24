import { Component, OnInit, TemplateRef } from '@angular/core';
import { Copy } from '../../_models/copy';
import { CopiesService } from '../../_services/copies.service';
import { PaginatedResponse, Pagination } from '../../_models/paginated-response';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { PaginationParams } from '../../_models/pagination-params';
import { ToastrService } from 'ngx-toastr';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CopyUpsertDto } from '../../_models/copy-upsert-dto';
import { ApiResponse } from '../../_models/api-response';

@Component({
  selector: 'app-admin-copies-list',
  templateUrl: './admin-copies-list.component.html',
  styleUrl: './admin-copies-list.component.css'
})
export class AdminCopiesListComponent implements OnInit {
  isbn: string | undefined;
  copies: Copy[] = {} as Copy[];
  pagination: Pagination | undefined;
  paginationParams: PaginationParams = new PaginationParams();
  modalRef: BsModalRef | undefined;
  constructor(
    private copiesService: CopiesService,
    private route: ActivatedRoute,
    private router: Router,
    private toAstr: ToastrService,
    private modalService: BsModalService) { }

  ngOnInit(): void {
    this.isbn = this.route.snapshot.params['isbn'];
    this.loadCopies();
  }

  loadCopies() {
    if (this.isbn) {
      this.copiesService.getCopies(this.isbn, this.paginationParams).subscribe({
        next: (response: PaginatedResponse<Copy[]>) => {
          if (response.result?.success) {
            this.copies = response.result.result;
            this.pagination = response.pagination;
          }
        }
      });
    }
  }

  pageChanged(event: any) {
    if (this.paginationParams && this.paginationParams.pageNumber !== event.page) {
      this.paginationParams.pageNumber = event.page;
      this.copiesService.setPagination(this.paginationParams);
      this.loadCopies();
    }
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  confirm(barcode: string): void {
    this.deleteCopy(barcode);
    this.modalRef?.hide();
  }

  decline(): void {
    this.modalRef?.hide()
  }

  deleteCopy(barcode: string) {
    this.copiesService.deleteCopy(barcode).subscribe({
      next: () => {
        this.toAstr.success("The copy was deleted successfuly", "Success");
        this.loadCopies();
      },
      error: () => {
        this.toAstr.error("An error ocurred while deleting the copy. Please try later","Error");
      }
    })
  }

  confirmReactivate(barcode: string): void {
    this.reactivateCopy(barcode);
    this.modalRef?.hide();
  }

  reactivateCopy(barcode: string) {
    //todo
  }

  createCopy(barcode: string) {
    if (!this.isbn) {
      this.toAstr.error('ISBN Not found', 'Error');
      return;
    }

    const copyCreateDto: CopyUpsertDto = {
      barcode: barcode,
      isbn: this.isbn
    }

    this.copiesService.createCopy(copyCreateDto).subscribe({
      next: response => {
        if (response.success) {
          this.toAstr.success('Copy created successfully', "Success");
          this.loadCopies();
        } else {
          this.toAstr.error('Failed to create copy','Error');
        }
      },
      error: () => {
        this.toAstr.error('Failed to create copy', 'Error');
      }
    })
  }

  UpdateCopy(barcode: string, newBarcode: string) {
    if (!this.isbn) {
      this.toAstr.error('ISBN Not found', 'Error');
      return;
    }

    const copyCreateDto: CopyUpsertDto = {
      barcode: newBarcode,
      isbn: this.isbn
    }

    this.copiesService.updateCopy(barcode, copyCreateDto).subscribe({
      next: response => {
        this.toAstr.success("Copy updated successfuly", "Success");
        this.loadCopies();
      },
      error: () => {
        this.toAstr.error("An error ocurred while updating the copy. Please try later","Error")
      }
    })
  }

}
