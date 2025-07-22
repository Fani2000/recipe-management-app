<template>
  <v-container v-if="recipe" fluid class="py-6" style="background-color: #fffaf4;">
    <v-img
      :src="recipe.imageUrl || 'https://source.unsplash.com/featured/?pasta,' + recipe.title"
      height="300px"
      cover
      class="rounded-lg mb-6"
    />

    <v-card elevation="6" rounded="xl" class="pa-6" style="background-color: #fffdf7;">
      <v-card-title class="text-h4 font-weight-bold mb-1">
        🍽️ {{ recipe.title }}
      </v-card-title>
      <v-card-subtitle class="mb-2 text-grey-darken-1">
        {{ recipe.description }}
      </v-card-subtitle>

      <div class="d-flex align-center mb-4">
        <v-icon color="orange darken-2" class="mr-2">mdi-timer</v-icon>
        <span class="text-subtitle-2 font-weight-medium orange--text">
          {{ recipe.cookingTimeMinutes }} minutes cooking time
        </span>
      </div>

      <v-divider class="my-4" />

      <v-card-text>
        <h4 class="text-h6 font-weight-bold mb-2">🧂 Ingredients</h4>
        <v-list dense class="mb-4">
          <v-list-item
            v-for="ingredient in recipe.ingredients?.$values || []"
            :key="ingredient.id"
          >
            <v-list-item-content>
              <v-icon start icon="mdi-circle-small" color="deep-orange" />
              {{ ingredient.content }}
            </v-list-item-content>
          </v-list-item>
        </v-list>

        <h4 class="text-h6 font-weight-bold mb-2 mt-6">👨‍🍳 Steps</h4>
        <v-list dense class="mb-4">
          <v-list-item
            v-for="(step, index) in recipe.steps?.$values || []"
            :key="step.id"
          >
            <v-list-item-content>
              <strong class="mr-2">Step {{ index + 1 }}:</strong>
              {{ step.content }}
            </v-list-item-content>
          </v-list-item>
        </v-list>

        <h4 class="text-h6 font-weight-bold mb-2 mt-6">🏷️ Tags</h4>
        <v-chip
          v-for="tag in recipe.tags?.$values || []"
          :key="tag.id"
          class="ma-1"
          color="pink lighten-4"
          text-color="pink darken-3"
          pill
          small
        >
          #{{ tag.name }}
        </v-chip>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
import { useRoute } from 'vue-router'
import { onMounted, ref } from 'vue'
import { useRecipeStore } from '@/stores/recipeStore'

const route = useRoute()
const store = useRecipeStore()
const recipe = ref<any>(null)

onMounted(async () => {
  recipe.value = await store.getRecipeById(Number(route.params.id))
})
</script>
