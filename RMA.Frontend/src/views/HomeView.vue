<template>
  <v-container>
    <v-text-field v-model="search" label="Search Recipes" clearable></v-text-field>

    <v-chip-group v-model="selectedTags" multiple>
      <v-chip v-for="tag in allTags" :key="tag" @click="toggleTag(tag)" :class="{ 'v-chip--active': selectedTags.includes(tag) }">
        {{ tag }}
      </v-chip>
    </v-chip-group>

    <v-row>
      <v-col v-for="recipe in filteredRecipes" :key="recipe.id" cols="12" sm="6" md="4">
        <v-card @click="goToRecipe(recipe.id)" class="cursor-pointer">
          <v-img :src="recipe.imageUrl" height="200px" />
          <v-card-title>{{ recipe.title }}</v-card-title>
          <v-card-subtitle>{{ recipe.description }}</v-card-subtitle>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useRecipeStore } from '@/stores/recipeStore'
import { onMounted, ref, watch } from 'vue'

const store = useRecipeStore()
const { recipes, filteredRecipes,} = storeToRefs(store)
const { fetchRecipes } = store

const search = ref('')
const selectedTags = ref<string[]>([])

onMounted(() => fetchRecipes())

// watch(search, (val) => store.setSearch(val))
// watch(selectedTags, (val) => store.setTags(val))

const router = useRouter()
const goToRecipe = (id: number) => router.push(`/recipe/${id}`)

const toggleTag = (tag: string) => {
  selectedTags.value.includes(tag)
    ? selectedTags.value.splice(selectedTags.value.indexOf(tag), 1)
    : selectedTags.value.push(tag)
}
</script>

<style scoped>
.cursor-pointer { cursor: pointer; }
.v-chip--active { background-color: #1976d2; color: white; }
</style>
