// src/services/recipeService.ts
import axios from 'axios'

const backendUrl = import.meta.env.BACKEND_URL || process.env.BACKEND_URL || ''
console.log("process", import.meta.env, " Other: ", process.env)
const API_URL = backendUrl + "/api/Recipes"
console.log("API_URL: ",  API_URL)

export const getRecipes = () => axios.get(API_URL)
export const getRecipeById = (id: number) => axios.get(`${API_URL}/${id}`)
export const createRecipe = (data: any) => axios.post(API_URL, data)
export const updateRecipe = (id: number, data: any) => axios.put(`${API_URL}/${id}`, data)
export const deleteRecipe = (id: number) => axios.delete(`${API_URL}/${id}`)
export const searchRecipes = (searchTerm: string, tags?: string[]) => axios.get(`${API_URL}/search=${searchTerm}&ascending=true&tags=${tags}`)
