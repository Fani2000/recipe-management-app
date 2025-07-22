// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import RecipeDetails from '../views/RecipeDetails.vue'

const routes = [
  { path: '/', name: 'Home', component: HomeView },
  { path: '/recipe/:id', name: 'RecipeDetails', component: RecipeDetails },
]

export default createRouter({
  history: createWebHistory(),
  routes,
})
