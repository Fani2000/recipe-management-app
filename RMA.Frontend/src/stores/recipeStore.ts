// src/stores/recipeStore.ts
import { defineStore } from 'pinia'
import * as api from '../services/recipeService'

export interface Recipe {
  id: number
  title: string
  description: string
  image: string
  tags: string[]
}

export const useRecipeStore = defineStore('recipe', {
  state: () => ({
    recipes: [] as Recipe[],
    loading: false,
    searchQuery: '',
    selectedTags: [] as string[],
  }),
  getters: {
    filteredRecipes(state): Recipe[] {
      return state.recipes.filter(r => {
        const matchesSearch = r.title.toLowerCase().includes(state.searchQuery.toLowerCase())
        const matchesTags = state.selectedTags.every(tag => r.tags.includes(tag))
        return matchesSearch && matchesTags
      })
    },
  },
  actions: {
    async fetchRecipes() {
      this.loading = true
      const { data } = await api.getRecipes()
      console.log('Fetched recipes:', data)
      this.recipes = data.$values
      this.loading = false
    },
    async getRecipeById(recipeId: any) {
      this.loading = true
      const { data } = await api.getRecipeById(recipeId)
      this.loading = false
      return data
    },
    async addRecipe(recipe: Omit<Recipe, 'id'>) {
      const { data } = await api.createRecipe(recipe)
      console.log('Added recipe:', data)
      this.recipes.push(data)
    },
    async updateRecipe(id: number, recipe: Partial<Recipe>) {
      const { data } = await api.updateRecipe(id, recipe)
      console.log('Added recipe:', data)
      const index = this.recipes.findIndex(r => r.id === id)
      if (index !== -1) this.recipes[index] = data
    },
    async deleteRecipe(id: number) {
      await api.deleteRecipe(id)
      this.recipes = this.recipes.filter(r => r.id !== id)
    },
  },
})
