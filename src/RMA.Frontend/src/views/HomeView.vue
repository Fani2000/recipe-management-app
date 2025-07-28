<template>
  <v-container fluid class="py-6" style="background: #fffaf4;overflow: hidden;height: 100vh;">
    <v-row justify="center" align="center" class="mb-0">
      <v-col cols="6">
        <v-text-field v-model="search" label="🔍 Find a delicious recipe..." variant="outlined" rounded
                      color="deep-orange" prepend-inner-icon="mdi-magnify" clearable />
      </v-col>
      <v-col cols="auto" class="mb-6">
        <v-btn color="deep-orange" dark @click="goToAddRecipe" prepend-icon="mdi-plus">
          Add Recipe
        </v-btn>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="4" class="rounded-xl" style="height: 84vh; overflow-y: scroll; background: white">
        <div class="text-h6 text-left text-bold text-grey mb-2">Recipe List: </div>

        <template v-for="recipe in filteredRecipes" :key="recipe.id">
          <v-hover v-slot="{ isHovering, props }">
            <v-card
              v-bind="props"
              :elevation="selectedRecipe?.id === recipe.id ? 6 : 1"
              :color="selectedRecipe?.id === recipe.id ? 'orange lighten-3' : ''"
              class="mb-2 cursor-pointer transition-all"
              @click="goToRecipe(recipe.id)"
              rounded
              border
              :border-color="selectedRecipe?.id === recipe.id ? 'orange lighten-4' : 'grey lighten-3'"
            >
              <template #prepend>
                <div class="d-flex justify-center items-center ga-3" style="height: 100%;">
                  <v-avatar size="90px" :image="recipe?.image" variant="flat" />
                  <div>
                    <v-card-title>{{ recipe.title }}</v-card-title>
                    <v-card-subtitle>
                      <v-chip
                        v-for="tag in recipe.tags?.$values || []"
                        :key="tag.id"
                        class="ma-1"
                        pill
                        small
                        @click.stop="toggleTag(tag.name)"
                      >
                        #{{ tag.name }}
                      </v-chip>
                    </v-card-subtitle>
                  </div>
                </div>
              </template>
            </v-card>
          </v-hover>
        </template>
      </v-col>

      <v-col cols="8" bg-color="white" class="rounded-xl" style="height: 84vh; overflow-y: scroll;">
        <RecipeDetails :selectedRecipe="selectedRecipe" v-show="selectedRecipe" />
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts" setup>
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useRecipeStore } from '@/stores/recipeStore'
import { onMounted, ref, watch } from 'vue'
import RecipeDetails  from '@/components/RecipeDetails.vue'

const store = useRecipeStore()
const { recipes, filteredRecipes, allTags } = storeToRefs(store)
const { fetchRecipes, setSearch, setTags } = store

const search = ref('')
const selectedTags = ref<string[]>([])
const selectedRecipe = ref<any>(null)
const router = useRouter()

onMounted(() => fetchRecipes())

// watch(search, (val) => store.setSearch(val))
// watch(selectedTags, (val) => store.setTags(val))

const goToRecipe = async (id: number) => {
  selectedRecipe.value = await store.getRecipeById(Number(id))
}

watch(search, (val) => setSearch(val) )

const goToAddRecipe = () => router.push('/add-recipe')

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
