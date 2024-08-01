export interface Book {
  title: string
  description: string
  isbn: string
  author: string
  imageUrl: string
  idCategory: number
  category: Category
}

export interface Category {
  id: number,
  name: string,
  description: string
}
