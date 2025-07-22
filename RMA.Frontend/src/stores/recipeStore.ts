import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import {
  getRecipes,
  getRecipeById as getRecipe,
  createRecipe,
  updateRecipe,
  deleteRecipe,
} from '@/services/recipeService'

export const useRecipeStore = defineStore('recipes', () => {
  const recipes = ref<any>([])
  const searchQuery = ref('')
  const selectedTags = ref<string[]>([])
  const loading = ref(false)

  const filteredRecipes = computed(() => {
    const data = recipes.value.$values ?? []
    return data.filter((r) => {
      const matchesSearch = r.title.toLowerCase().includes(searchQuery.value.toLowerCase())
      const matchesTags =
        selectedTags.value.length === 0 ||
        (r.tags?.$values || []).some((tag: any) => selectedTags.value.includes(tag.name))
      return matchesSearch && matchesTags
    })
  })

  const allTags = computed(() => {
    const tagSet = new Set<string>()
    recipes.value.forEach((r) => (r.tags?.$values || []).forEach((t: any) => tagSet.add(t.name)))
    return Array.from(tagSet).map((name) => ({ name }))
  })

  const fetchRecipes = async () => {
    loading.value = true
    const { data } = await getRecipes()
    recipes.value = data
    loading.value = false
  }

  const setSearch = (query: string) => {
    searchQuery.value = query
  }
  const setTags = (tags: string[]) => {
    selectedTags.value = tags
  }

  const getRecipeById: any = async (id: number) => {
    loading.value = true
    const { data } = await getRecipe(id)
    console.log('Recipe fetched:', data)
    loading.value = false
    return data
  }

  const addRecipe = async (recipe: any) => {
    const { data } = await createRecipe(recipe)
    recipes.value.push(data)
  }

  const modifyRecipe = async (id: number, recipe: any) => {
    const { data } = await updateRecipe(id, recipe)
    const index = recipes.value.findIndex((r) => r.id === id)
    if (index !== -1) recipes.value[index] = data
  }

  const removeRecipe = async (id: number) => {
    await deleteRecipe(id)
    recipes.value = recipes.value.filter((r) => r.id !== id)
  }

  return {
    recipes,
    filteredRecipes,
    allTags,
    fetchRecipes,
    getRecipeById,
    addRecipe,
    modifyRecipe,
    removeRecipe,
    setSearch,
    setTags,
    loading,
  }
})
