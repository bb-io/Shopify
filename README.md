# Blackbird.io Shopify

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Shopify is a cloud-based e-commerce platform that enables users to create, customize, and manage online stores. It provides a wide range of features and tools for businesses to sell products and services online, including website templates, payment processing, inventory management, and marketing tools. Shopify allows users to reach customers across different channels, including web, mobile, social media, and marketplaces, making it a versatile solution for individuals, entrepreneurs, and enterprises looking to establish and grow their online presence.

## Connecting

1. In Blackbird, navigate to 'Apps' and search for Shopify. If you cannot find Shopify then click _Add App_ in the top right corner, select Shopify and add the app to your Blackbird environment.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Shopify connection'.
4. In Shopify, go to `Store settings -> Apps and sales channels -> Develop apps`.
5. Create the develop app.
6. Click `Configure Admin API scopes` to select the scopes you want to provide Blackbird with. Make sure to provide `read_locales` access scope. Other scopes that may be needed depending on desired actions are: `write_products`, `read_products`, `write_publications`, `read_publications`, `write_translations`, `read_translations`.
7. After selecting the scopes click `Save` and go to `API credentials` and install the newly created app.
8. It will create the `Admin API access token` that you need to copy and paste it to the appropriate field in Blackbird.
9. Next in store settings click `Domains` and copy the name of the store from the domain and paste it to the appropriate field in Blackbird. F.E. if your domain is `mystore.myshopify.com`, you need to take `mystore`
10. Click _Connect_.
11. Confirm that the connection has appeared and the status is _Connected_. 

## Actions

### Products

- **Search products** returns the list of products filtered by provided criterias.
- **Get product content as HTML** returns source product content in an HTML format.
- **Get product translation content as HTML** returns translated product content in an HTML format.
- **Update product content from HTML** updates content of the specified product from an HTML file.

## Events

- **On product created**
- **On product deleted**
- **On product updated**
- **On product publications added**
- **On product publications deleted**

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
