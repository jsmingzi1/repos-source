
#ifndef UTIL_H
#define UTIL_H



typedef struct map {
	unsigned long key;
	void* value;
} map, *pmap;

typedef struct map_node {
	map* current;
	struct map_node* next;
} map_node, *pmap_node;

void* get_mapchain_value(pmap_node, unsigned long );
int  exist_in_mapchian(pmap_node, unsigned long );
pmap_node  add_to_mapchain(pmap_node, unsigned long key, void *);
int  update_to_mapchain(pmap_node, unsigned long , void* );
pmap_node  remove_from_mapchain(pmap_node, unsigned long );
int  get_count_mapchain(pmap_node);

#endif
