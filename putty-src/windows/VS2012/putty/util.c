#include "util.h"
#include <stdio.h>
#include <stdlib.h>
#define NULL 0

void* get_mapchain_value(pmap_node chain, unsigned long key) {
	pmap_node node = chain;
	while (node) {
		if (node->current->key == key)
			return node->current->value;
		node = node->next;
	}

	return NULL;
}

int  exist_in_mapchian(pmap_node chain, unsigned long key) {
	pmap_node node = chain;
	while (node) {
		if (node->current->key == key)
			return 1;
		node = node->next;
	}

	return 0;
}

//add to chain, also can be used as update value, if the input is empty, will create first one
pmap_node  add_to_mapchain(pmap_node chain, unsigned long key, void* value) {
	pmap_node node = chain;
	if (node) {
		do {
			if (node->current->key == key) {
				node->current->value = value;
				return node;
			}

			if (node->next == NULL)
				break;
			node = node->next;
		} while (1);
	}

	pmap_node newnode = calloc(1, sizeof(map_node));
	if (newnode == NULL)
		return NULL;
	newnode->current = (pmap)calloc(1, sizeof(map));
	if (newnode->current == NULL)
		return NULL;
	newnode->current->key = key;
	newnode->current->value = value;
	if (node)
		node->next = newnode;
	else
		node = newnode;

	return node;
}

int  update_to_mapchain(pmap_node chain, unsigned long key, void* value) {
	return add_to_mapchain(chain, key, value);
}

pmap_node  remove_from_mapchain(pmap_node chain, unsigned long key) {
	pmap_node node = chain;
	pmap_node node_pre = chain;
	while (node) {
		if (node->current->key == key) {
			free(node->current);
			if (node != node_pre) {
				node_pre->next = node->next;
				free(node);
				return node_pre;
			}
			else {
				node_pre = node->next;
				free(node);
				return node_pre;
			}
		}
		node_pre = node;
		node = node->next;
	}
	return node_pre;
}

int  get_count_mapchain(pmap_node chain)
{
	int count = 0;
	pmap_node node = chain;
	while (node) {
		node = node->next;
		count++;
	}
	return count;
}

