int imprimePilhas(Elemento* pilha[3]) {
    printf("\n\n\t\t     1    2    3\n\n");

    Elemento* atual[3];
    for (int i = 0; i < 3; i++)
        atual[i] = pilha[i]->topo;

    imprimeRecursivo(atual, 5);
}

int imprimeRecursivo(Elemento* pilha[3], int size) {
    if (size == 0) return;

    char temp[3];
    for (int i = 0; i < 3; i++) {
        if (pilha[i] != null) {
            pilha[i] = pilha[i]->dado;
            temp[i] = temp[i]->proximo;
        } else
            temp[i] = ' ';
    }
    func(pilha, --size);
    printf("\t\t    |%c|    |%c|    |%c|\n", temp[0], temp[1], temp[2]);
}
