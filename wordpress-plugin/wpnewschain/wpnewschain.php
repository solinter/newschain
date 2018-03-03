<?php
/**
 * Plugin Name: NewsChain
 * Plugin URI: http://newschain.es
 * Description: Este plugin es para el hackethon
 * Version: 1.0.0
 * Author: Samuel Garcia
 * Author URI: 
 * 
 * Domain Path: /languages/
 */
defined( 'ABSPATH' ) or die( '¡Sin trampas!' );

function my_project_updated_send_post( $post_id ) {

	// obtener el hash 
		$var_content = get_the_content();
		// esta variable a una array
		$data_to_send = array();
		$data_to_send["content"] = $var_content;
		$data_to_send["url"] = get_permalink($post_id);
		$url = "http://5.9.131.156:8080";
	
	$response = wp_remote_post( $url, array(
		'method' => 'POST',
		'timeout' => 45,
		'redirection' => 5,
		'httpversion' => '1.0',
		'blocking' => true,
		'headers' => array(),
		'body' => $data_to_send,
		'cookies' => array()
		)
	);

	if ( is_wp_error( $response ) ) {
	   $error_message = $response->get_error_message();
	   echo "Something went wrong: $error_message";
	} else {
	   
	   print_r( $response["contract_id"] );
	   update_post_meta($post_id , "contract_id" ,$response["contract_id"]);
	}
}
add_action( 'save_post', 'my_project_updated_send_post' );