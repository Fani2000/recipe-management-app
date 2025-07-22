<template>
  <v-container fluid class="py-6" style="background: #fffaf4;">
    <v-row justify="center" class="mb-6">
      <v-col cols="12" md="6">
        <v-text-field
          v-model="search"
          label="🔍 Find a delicious recipe..."
          variant="outlined"
          rounded
          color="deep-orange"
          prepend-inner-icon="mdi-magnify"
          clearable
        />
      </v-col>
    </v-row>

    <v-row justify="center" class="mb-4">
      <v-chip-group v-model="selectedTags" multiple column>
        <v-chip
          v-for="tag in allTags"
          :key="tag.name"
          @click="toggleTag(tag.name)"
          :color="selectedTags.includes(tag.name) ? 'deep-orange accent-3' : 'orange lighten-4'"
          class="ma-1"
          pill
          elevated
        >
          {{ tag.name }}
        </v-chip>
      </v-chip-group>
    </v-row>

    <v-row>
      <v-col
        v-for="recipe in filteredRecipes"
        :key="recipe.id"
        cols="12"
        sm="6"
        md="4"
        class="d-flex"
      >
        <v-hover v-slot="{ isHovering, props }">
          <v-card
            v-bind="props"
            @click="goToRecipe(recipe.id)"
            class="cursor-pointer transition-smooth"
            :elevation="isHovering ? 12 : 4"
            rounded="xl"
            style="width: 100%; background-color: #fffbe6;"
          >
            <v-img
              :src="recipe?.image || 'https://source.unsplash.com/featured/?food,' + recipe.title"
              height="200"
              cover
              class="rounded-t-xl"
            />
            <v-card-title class="text-h6 font-weight-bold px-4 pt-3">
              🍝 {{ recipe.title }}
            </v-card-title>
            <v-card-subtitle class="px-4 pb-2 text-grey-darken-1">
              {{ recipe.description }}
            </v-card-subtitle>

            <v-card-text class="px-4 pb-2">
              <v-chip
                v-for="tag in recipe.tags?.$values || []"
                :key="tag.id"
                class="ma-1"
                color="pink lighten-4"
                text-color="pink darken-3"
                pill
                small
                @click.stop="toggleTag(tag.name)"
              >
                #{{ tag.name }}
              </v-chip>
            </v-card-text>

            <div class="px-4 pb-4 text-caption text-right text-deep-orange-darken-4">
              ⏱️ {{ recipe.cookingTimeMinutes }} minutes
            </div>
          </v-card>
        </v-hover>
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
const { recipes, filteredRecipes } = storeToRefs(store)
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
.cursor-pointer {
  cursor: pointer;
}

.v-chip--active {
  background-color: #1976d2;
  color: white;
}
</style>