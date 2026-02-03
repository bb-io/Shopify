# Blackbird.io Shopify

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Shopify is a cloud-based e-commerce platform that enables users to create, customize, and manage online stores. It provides a wide range of features and tools for businesses to sell products and services online, including website templates, payment processing, inventory management, and marketing tools. Shopify allows users to reach customers across different channels, including web, mobile, social media, and marketplaces, making it a versatile solution for individuals, entrepreneurs, and enterprises looking to establish and grow their online presence.

## Connecting

1. In Blackbird, navigate to 'Apps' and search for Shopify.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Shopify connection'.
4. In Shopify, go to `Store settings -> Apps and sales channels -> Develop apps`.
5. Create the develop app.
6. Click `Configure Admin API scopes` to select the scopes you want to provide Blackbird with. Make sure to provide `read_locales` access scope. Other scopes that may be needed depending on desired actions are: `write_products`, `read_products`, `write_publications`, `read_publications`, `write_translations`, `read_translations`, `read_themes`, `read_content`.
7. After selecting the scopes click `Save` and go to `API credentials` and install the newly created app.
8. It will create the `Admin API access token` that you need to copy and paste it to the appropriate field in Blackbird.
9. Next in store settings click `Domains` and copy the name of the store from the domain and paste it to the appropriate field in Blackbird. F.E. if your domain is `mystore.myshopify.com`, you need to take `mystore`
10. Click _Connect_.
11. Confirm that the connection has appeared and the status is _Connected_. 

## Actions

### Store

- **Get store locales information** Get primary and additional store locales
- **Download store resources** Download content of all store resource type items
- **Upload store resources** Upload content of all store resource type items
- **Download store content** Download content of the store. You should specify what resources you want to export: theme, menu, shop or shop policy.
- **Upload store content** Upload content of the store

### Articles

- **Search articles** Search articles with specific criteria
- **Download article** Download content of a specific article
- **Upload article** Upload content of a specific article

### Blogs

- **Search blogs** Search blogs with specific criteria
- **Download blog** Download content of a specific blog
- **Upload blog** Upload content of a specific blog

### Pages

- **Search pages** Search pages with specific criteria
- **Download page** Download content of a specific page
- **Upload page** Upload content of a specific page

### Products

- **Search products** Search products with specific criteria
- **Download product** Download content of a specific product
- **Uploade product** Upload content of a specific product

### Metafields

- **Download metafields** Download metafield content of a specific product
- **Upload metafields** Upload metafield content of a specific product
- **Search metafield definitions** Search metafield definitions with specific criteria
- **Get metafield** Get metafield details of a specific product
- **Update metafield** Update metafield value of a specific product

### Themes

- **Download heme** Download content of a specific theme
- **Upload theme** Upload content of a specific theme
- **Search themes** Search themes with specific criteria

### Content

> **Note**: These actions support the following content types: collection, metafield, article, blog, page, theme, product.

- **Upload content** Upload content of a specific content type from a file
- **Download content** Download content of a specific content type
- **Search content** Search through different content types with specific criteria

## Events

### Articles

- **On articles created**
- **On articles updated**

### Pages

- **On pages created**
- **On pages updated**

### Products

- **On product created**
- **On product deleted**
- **On product updated**
- **On product publications added**
- **On product publications deleted**

### Locales

- **On locale created**
- **On locale updated**

### Content

> **Note**: These events support the following content types: collection, article, blog, page, product.

- **On content updated**

## Example

The following example shows how a bird can be setup so that anytime a Shopify product is updated, we get the locale setup of the online store, download the source content for the updated product as an HTML file, translate the file into all target locales using GlobalLink NOW MT solution and update all target products.

![Sample Bird](image/README/ShopifySampleBird.png)

## Demo video

Shopify + DeepL + OpenAI: Multilingual Products in Minutes

import { YouTube } from '@astro-community/astro-embed-youtube';

<YouTube id="https://youtu.be/z7ILq75EVcY" />

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
