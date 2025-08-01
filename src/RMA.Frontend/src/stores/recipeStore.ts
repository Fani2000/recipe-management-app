import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import {
  getRecipes,
  getRecipeById as getRecipe,
  createRecipe,
  updateRecipe,
  deleteRecipe,
  searchRecipes,
} from '@/services/recipeService'

export const useRecipeStore = defineStore('recipes', () => {
  const recipes = ref<any>([])
  const searchQuery = ref('')
  const selectedTags = ref<string[]>([])
  const loading = ref(false)

  const filteredRecipes = computed(() => {
    const data = recipes.value ?? []
    return data.filter((r: any) => {
      // console.log("Filteted recipes: ", r.title, "Search: ", searchQuery.value, "Tags: ", selectedTags.value, "")
      const matchesSearch = r.title?.toLowerCase().includes(searchQuery.value.toLowerCase())
      const matchesTags =
        selectedTags.value.length === 0 ||
        (r.tags?.$values || []).some((tag: any) => selectedTags.value.includes(tag.name))
      return matchesSearch && matchesTags
    })
  })

  const allTags = computed(() => {
    const tagSet = new Set<string>()
    const data = recipes.value ?? []
    // console.log('recipes', data)
    data.forEach((r: any) => (r.tags?.$values || []).forEach((t: any) => tagSet.add(t.name)))
    return Array.from(tagSet).map((name) => ({ name }))
  })

  const fetchRecipes = async () => {
    loading.value = true
    const { data } = await getRecipes()
    recipes.value = data.$values
    // console.log('Recipes fetched:', data, " Recipes: ", recipes.value)
    loading.value = false
  }

  const setSearch = (query: string) => {
    searchQuery.value = query
    console.log("Search: ", searchQuery.value)
    search()
  }
  const setTags = (tags: string[]) => {
    selectedTags.value = tags
  }

  const search = async () => {
    loading.value = true
    const { data } = await searchRecipes(searchQuery.value, selectedTags.value)
    recipes.value = data.$values
    // console.log('Recipes fetched:', data, " Recipes: ", recipes.value)
    loading.value = false
  }

  const getRecipeById: any = async (id: number) => {
    loading.value = true
    const { data } = await getRecipe(id)
    // console.log('Recipe fetched:', data)
    loading.value = false
    return data
  }

  const addRecipe = async (recipe: any) => {
    const { data } = await createRecipe(recipe)
    // console.log('New recipe added:', data, "Recipes ",recipes.value)
    recipes.value.push(data)
  }

  const modifyRecipe = async (id: number, recipe: any) => {
    // console.log('Updating recipe with ID:', id, 'Data:', recipe)
    loading.value = true
    const { data } = await updateRecipe(id, { ...recipe, id })
    // console.log(" recipes: ", recipes.value)
    const index = recipes.value.findIndex((r: any) => r.id === id)
    if (index !== -1) recipes.value[index] = data
  }

  const removeRecipe = async (id: number) => {
    await deleteRecipe(id)
    // console.log(recipes.value)
    recipes.value = recipes.value.filter((r: any) => r.id !== id)
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
