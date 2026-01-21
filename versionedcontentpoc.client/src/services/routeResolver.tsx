import type { Language } from "../api/client";

type Param<K extends string> = K | `${K}?`;

class Route<P extends Record<string, unknown>> {
    constructor(
        public readonly route: string,
        public readonly params: readonly Param<keyof P & string>[]
    ) { }

    pattern(): string {
        return [
            this.route,
            ...this.params
        ].join("/:");
    }

    build(values: P): string {
        const parts = this.params
            .map(p => values[p.replace("?", "") as keyof P])
            .filter(v => v !== undefined)
            .map(String);

        return [this.route, ...parts].join("/");
    }
}

export const routes = {
    home: '/',
    cmshome: '/cms',
    create: '/cms/create',
    edit: new Route<{ contentId: string; language: Language; versionId?: string }>(
        '/cms/edit',
        ['contentId', 'language', 'versionId?']
    ),
}
