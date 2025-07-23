// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import RecipeDetails from '../views/RecipeDetails.vue'
import AddRecipe from '../views/AddRecipe.vue'
import EditRecipe from '../views/EditRecipe.vue'

const routes = [
  { path: '/', name: 'Home', component: HomeView },
  { path: '/recipe/:id', name: 'RecipeDetails', component: RecipeDetails },
  { path: '/edit/:id', name: 'Edit Recipe', component: EditRecipe },
  { path: '/add-recipe', name: 'Add new Recipe', component: AddRecipe },
]

export default createRouter({
  history: createWebHistory(),
  routes,
})
